#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.TestRunners.Common;
using Microsoft.DotNet.XHarness.TestRunners.Xunit;
using Xunit;

namespace Microsoft.Maui.TestUtils.DeviceTests.Runners.HeadlessRunner
{
	public class ControlsHeadlessTestRunner : AndroidApplicationEntryPoint
	{
		const string CategoriesFileName = "devicetestcategories.txt";
		const string FilterFileName = "devicetestfilter.txt";
		readonly string _categoriesFilePath;
		readonly string _filterFilePath;

		public static string? TestResultsFile;
		public static int? LoopCount;

		readonly HeadlessRunnerOptions _runnerOptions;
		readonly TestOptions _options;
		string? _resultsPath;
		readonly int _loopCount;
		TestLogger _logger;

		public ControlsHeadlessTestRunner(HeadlessRunnerOptions runnerOptions, TestOptions options)
		{
			_runnerOptions = runnerOptions;
			_options = options;
			_resultsPath = TestResultsFile;
			var resultsDir = Path.GetDirectoryName(_resultsPath) ?? string.Empty;
			_categoriesFilePath = Path.Combine(resultsDir, CategoriesFileName);
			_filterFilePath = Path.Combine(resultsDir, FilterFileName);
			_loopCount = LoopCount ?? 0;
			_logger = new();
		}

		protected override bool LogExcludedTests => true;

		public override TextWriter? Logger => _logger;

		public override string TestsResultsFinalPath => _resultsPath!;

		protected override int? MaxParallelThreads => System.Environment.ProcessorCount;

		protected override IDevice Device { get; } = new TestDevice();

		protected override IEnumerable<TestAssemblyInfo> GetTestAssemblies() =>
			_options.Assemblies
				.Distinct()
				.Select(assembly => new TestAssemblyInfo(assembly, assembly.Location));

		protected override void TerminateWithSuccess()
		{
			UI.Xaml.Application.Current.Exit();
		}

		protected override TestRunner GetTestRunner(LogWriter logWriter)
		{
			var testRunner = base.GetTestRunner(logWriter);

			// Filter mode: run only specific classes/methods from the filter file
			if (_loopCount == -2 && File.Exists(_filterFilePath))
			{
				var filterLines = File.ReadAllLines(_filterFilePath)
					.Where(l => !string.IsNullOrWhiteSpace(l))
					.ToList();

				var classFilters = filterLines
					.Where(l => l.StartsWith("class:"))
					.Select(l => l.Substring("class:".Length).Trim())
					.ToList();

				var methodFilters = filterLines
					.Where(l => l.StartsWith("method:"))
					.Select(l => l.Substring("method:".Length).Trim())
					.ToList();

				if (classFilters.Count > 0 || methodFilters.Count > 0)
				{
					testRunner.RunAllTestsByDefault = false;

					foreach (var cls in classFilters)
					{
						testRunner.SkipClass(cls, false);
					}

					foreach (var method in methodFilters)
					{
						testRunner.SkipMethod(method, false);
					}
				}

				// Use a single result file (no category suffix) for filter mode
				var resultPath = _resultsPath?.Split(".xml") ?? new[] { "" };
				_resultsPath = $"{resultPath[0]}_filtered.xml";

				return testRunner;
			}

			var allCategories = File.ReadAllLines(_categoriesFilePath);
			var categoriesToRun = allCategories.Skip(_loopCount).Take(1).ToArray();

			List<string> categoriesToSkip = new();

			foreach (var test in allCategories.Except(categoriesToRun))
			{
				categoriesToSkip.Add($"Category={test}");
			}

			var currentCategory = categoriesToRun[0];
			var resultPathCat = _resultsPath?.Split(".xml") ?? new[] { "" };
			_resultsPath = $"{resultPathCat[0]}_{currentCategory}.xml";

			testRunner.SkipCategories(categoriesToSkip);

			return testRunner;
		}

		public async Task<string?> RunTestsAsync()
		{
			TestsCompleted += OnTestsCompleted;

			try
			{
				// Discovery mode: find all categories and write them to file
				if (_loopCount == -1)
				{
					var categories = DiscoverTestsInAssemblies();
					File.WriteAllLines(_categoriesFilePath, categories.ToArray());

					TerminateWithSuccess();
					return null;
				}

				// Filter mode: run only specific classes/methods
				// Category mode: run category at index _loopCount
				// Both use RunAsync — GetTestRunner applies the appropriate filters
				await RunAsync();
			}
			catch (Exception ex)
			{
				_logger.WriteLine(ex.ToString());
			}
			TestsCompleted -= OnTestsCompleted;

			if (File.Exists(TestsResultsFinalPath))
			{
				return TestsResultsFinalPath;
			}

			return null;

			void OnTestsCompleted(object? sender, TestRunResult results)
			{
				var message =
					$"Tests run: {results.ExecutedTests} " +
					$"Passed: {results.PassedTests} " +
					$"Inconclusive: {results.InconclusiveTests} " +
					$"Failed: {results.FailedTests} " +
					$"Ignored: {results.SkippedTests}";

				_logger.WriteLine("test-execution-summary" + message);
				_logger.WriteLine("return-code " + (results.FailedTests == 0 ? 0 : 1));
			}
		}

		IEnumerable<string> DiscoverTestsInAssemblies()
		{
			var result = new List<string>();

			try
			{
				foreach (var assm in GetTestAssemblies())
				{
					var nameWithoutExt = assm.Assembly.GetName().Name;
					var assemblyFileName = Storage.FileSystemUtils.PlatformGetFullAppPackageFilePath($"{nameWithoutExt}.dll");

					var discoveryOptions = TestFrameworkOptions.ForDiscovery();

					try
					{
						using (var framework = new XunitFrontController(AppDomainSupport.Denied, assemblyFileName, null, false))
						using (var sink = new TestDiscoverySink())
						{
							framework.Find(false, sink, discoveryOptions);
							sink.Finished.WaitOne();

							result.AddRange(sink.TestCases.SelectMany(tc => tc.Traits["Category"]).Distinct());
						}
					}
					catch (Exception e)
					{
						Debug.WriteLine(e);
					}
				}
			}
			catch (Exception e)
			{
				Debug.WriteLine(e);
			}

			return result;
		}
	}
}