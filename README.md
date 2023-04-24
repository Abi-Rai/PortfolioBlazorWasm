# Blazor Webassembly Project for Portfolio

I built this application to learn Blazor.

`Blazor Webassembly version 7.0.4, .NET 7.0`

## Live demo 

Github Pages :rocket: : https://abi-rai.github.io/PortfolioBlazorWasm/ 

## Pages

The project contains several pages where each page addresses some of the core web development framework concepts such as:
- State Management 
- Data Persistence 
- Dependency injection
- Routing
- Testing
- Data Binding

## Running locally
You can run locally using either Visual Studio or Docker Desktop. Running in development mode using visual studio is a bit laggy. Especially when rendering a page with lots of css animations.


### 1. Visual Studio 2022

```
1. Clone the repository 
2. Open the BlazorWasmPortfolio.sln in Visual Studio
3. Set startup project as PortfolioBlazorWasm
4. Build and run the project
```

### 2. Docker Desktop

To build image & run the container

1. :exclamation:  Change directory to the WASM Client project :exclamation:
	```shell
	cd PortfolioBlazorWasm
	```
2. Use docker-compose
	```shell
	docker-compose up --build
	```
After it finishes building, the site will be live on `localhost:8080`

3. `Ctrl + C` in terminal to stop the container.

4. Then to delete the container.
	```shell
	 docker-compose down
	```

## Credits

- [MudBlazor](https://github.com/MudBlazor/MudBlazor/) : Blazor UI component Library
- [Blazor-ApexCharts](https://github.com/apexcharts/Blazor-ApexCharts) : ApexCharts wrapper for Blazor
- [Pathfinding Visualizer](https://github.com/clementmihailescu/Pathfinding-Visualizer) : Where I got the idea for building the pathfinding page.
