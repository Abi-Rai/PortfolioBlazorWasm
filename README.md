# Blazor Webassembly Project for Portfolio

I built this application to learn Blazor.

`Blazor Webassembly version 7.0.4, .NET 7.0`

## Live demo

Github Pages : https://abi-rai.github.io/PortfolioBlazorWasm/

## Pages

The project contains different types of pages where each page addresses some of the core web development framework concepts such as:
- State Management 
- Data Persistence 
- Dependency injection
- Testing
- Routing

## Running locally

### Visual Studio 2022

```
1. Clone the repository 
2. Open the BlazorWasmPortfolio.sln in Visual Studio
3. Set startup project as PortfolioBlazorWasm
4. Build and run the project
```

### Docker Desktop

To build image & run the container
  
```shell
cd PortfolioBlazorWasm
docker-compose up --build
```
After it finishes building, the site will be live on `localhost:8080`


>`Ctrl + C` to stop the container.

Then to delete the container.
```shell
 docker-compose down
```

## Credits

- [MudBlazor](https://github.com/MudBlazor/MudBlazor/) : Blazor UI component Library
- [Blazor-ApexCharts](https://github.com/apexcharts/Blazor-ApexCharts) : ApexCharts wrapper for Blazor
- [Pathfinding Visualizer](https://github.com/clementmihailescu/Pathfinding-Visualizer) : Where I got the idea for building the pathfinding page.
