using Moq;
using PortfolioBlazorWasm.Models.Pathfinding;
using Xunit;
using FluentAssertions;
using PortfolioBlazorWasm.Models.Pathfinding.Enums;
using PortfolioBlazorWasm.Services.Pathfinding.Algorithms;
using PortfolioBlazorWasm.Services.Pathfinding;

namespace PortfolioBlazorWasm.Tests.Services.Pathfinding
{
    public class PathfindingRunnerTests
    {

        [Fact]
        public async Task GenerateAndSetGridAsync_WhenCreatedWithValidParamters_ShouldAssignGridAndStartAndFinishNodes()

        {
            // Arrange
            var runner = new PathfindingRunner();
            int row = 20;
            int col = 30;

            // Act
            await runner.GenerateAndSetGridAsync(row, col);

            // Assert
            runner.Grid.Should().NotBeNull();
            runner.Grid.GetLength(0).Should().Be(row);
            runner.Grid.GetLength(1).Should().Be(col);

            runner.Grid[9, 10].State.Should().Be(NodeState.Start);
            runner.Grid[9, 15].State.Should().Be(NodeState.Finish);

        }

        [Fact]
        public async Task RunAlgorithm_WithCancellationRequested_ShouldThrowOperationCanceledException()
        {
            // Arrange
            var searchSettings = new SearchSettings { AlgorithmType = AlgorithmTypes.Ddijkstras, SearchSpeed = SearchSpeeds.Slow };
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            var runner = new PathfindingRunner();

            // Act
            await runner.GenerateAndSetGridAsync(20, 30);
            Func<Task> act = async () => await runner.RunAlgorithm(searchSettings, cancellationTokenSource.Token);

            // Assert
            await act.Should().ThrowAsync<OperationCanceledException>();
        }
        [Fact]
        public async Task RunAlgorithm_WithDdijkstrasAlgorithmType_ShouldSetChosenAlgorithmToDdijkstras()
        {
            // Arrange
            var searchSettings = new SearchSettings { AlgorithmType = AlgorithmTypes.Ddijkstras, SearchSpeed = SearchSpeeds.Slow };
            var cancellationToken = new CancellationToken();
            var runner = new TestablePathfindingRunner();

            // Act
            await runner.RunAlgorithm(searchSettings, cancellationToken);

            // Assert
            runner.ChosenAlgorithm.Should().BeSameAs(runner.MockAlgorithm.Object);
        }
        [Fact]
        public async Task ClearGrid_WithShouldClearWallsTrue_ShouldClearVisitedAndWalls()
        {
            // Arrange
            var runner = new PathfindingRunner();
            await runner.GenerateAndSetGridAsync(10, 20);
            runner.Grid[1, 1].State = NodeState.Wall;
            runner.Grid[0, 1].Visited = true;

            // Act
            await runner.ClearGrid(shouldClearWalls: true);

            // Assert
            int startCount = 0;
            int finishCount = 0;
            int noneCount = 0;
            for (int i = 0; i < runner.Grid.GetLength(0); i++)
            {
                for (int j = 0; j < runner.Grid.GetLength(1); j++)
                {
                    Node node = runner.Grid[i, j];
                    node.Visited.Should().BeFalse();
                    switch (node.State)
                    {
                        case NodeState.Start:
                            startCount++;
                            break;
                        case NodeState.Finish:
                            finishCount++;
                            break;
                        case NodeState.None:
                            noneCount++;
                            break;
                    }
                }
            }
            startCount.Should().Be(1);
            finishCount.Should().Be(1);
            noneCount.Should().Be(runner.Grid.Length - 2);
        }
        [Fact]
        public async Task ClearGrid_WithShouldClearWallsFalse_ShouldClearVisitedAndShortestPathsButNotWalls()
        {
            // Arrange
            var runner = new PathfindingRunner();
            await runner.GenerateAndSetGridAsync(10, 20);
            runner.Grid[1, 1].State = NodeState.Wall;
            runner.Grid[0, 1].Visited = true;
            runner.Grid[1, 2].State = NodeState.ShortestPath;
            // Act
            await runner.ClearGrid(shouldClearWalls: false);

            // Assert
            int startCount = 0;
            int finishCount = 0;
            int wallCount = 0;
            int noneCount = 0;
            for (int i = 0; i < runner.Grid.GetLength(0); i++)
            {
                for (int j = 0; j < runner.Grid.GetLength(1); j++)
                {
                    Node node = runner.Grid[i, j];
                    node.Visited.Should().BeFalse();
                    switch (node.State)
                    {
                        case NodeState.Start:
                            startCount++;
                            break;
                        case NodeState.Finish:
                            finishCount++;
                            break;
                        case NodeState.Wall:
                            wallCount++;
                            break;
                        case NodeState.None:
                            noneCount++;
                            break;
                    }
                }
            }
            startCount.Should().Be(1);
            finishCount.Should().Be(1);
            wallCount.Should().Be(1);
            noneCount.Should().Be(runner.Grid.Length - 3);
        }
        [Fact]
        public async Task RaiseVisitedEvent_ShouldInvokeVisitedChangedEventWithCorrectArguments()
        {
            // Arrange
            var runner = new PathfindingRunner();
            object? eventSender = null;
            EventArgs? eventArgs = null;
            runner.VisitedChanged += (sender, e) =>
            {
                eventSender = sender;
                eventArgs = e;
                return Task.CompletedTask;
            };
            var expectedSender = this;
            var expectedEventArgs = EventArgs.Empty;

            // Act
            await runner.RaiseVisitedEvent(expectedSender, expectedEventArgs);

            // Assert
            eventSender.Should().Be(expectedSender);
            eventArgs.Should().Be(expectedEventArgs);
        }

        [Fact]
        public async Task RaiseShortestFound_ShouldInvokeShortestFoundEventWithCorrectArguments()
        {
            // Arrange
            var runner = new PathfindingRunner();
            object? eventSender = null;
            Stack<Node>? eventArgs = null;
            runner.ShortestFound += (sender, e) =>
            {
                eventSender = sender;
                eventArgs = e;
                return Task.CompletedTask;
            };
            var expectedSender = this;
            var expectedEventArgs = new Stack<Node>();

            // Act
            await runner.RaiseShortestFound(expectedSender, expectedEventArgs);

            // Assert
            eventSender.Should().Be(expectedSender);
            eventArgs.Should().BeSameAs(expectedEventArgs);
        }
        public class TestablePathfindingRunner : PathfindingRunner
        {
            public Mock<IAlgorithm> MockAlgorithm { get; } = new Mock<IAlgorithm>();
            protected override IAlgorithm CreateAlgorithm(AlgorithmTypes algorithmType)
            {
                return MockAlgorithm.Object;
            }
        }
    }
}
