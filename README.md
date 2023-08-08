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

1. Clone the repository

2. :exclamation: Change directory to the WASM Client project :exclamation:
   ```shell
   cd PortfolioBlazorWasm
   ```
3. Use docker-compose
   ```shell
   docker-compose up --build
   ```

   > After it finishes building, the site will be live on `localhost:8080`

4. `Ctrl + C` in terminal to stop the container.

5. Then to delete the container.
   ```shell
    docker-compose down
   ```

---

### Images

<img src="https://github.com/Abi-Rai/PortfolioBlazorWasm/assets/52832186/1b9254e2-b360-4dd9-94f6-56f8f136e3f1" width="46%"></img>
<img src="https://github.com/Abi-Rai/PortfolioBlazorWasm/assets/52832186/bd2cab2f-026d-4483-929f-6d541a0fc10f" width="37.5%"></img>
<img src="https://github.com/Abi-Rai/PortfolioBlazorWasm/assets/52832186/36a5c98f-a1f0-441e-8561-3d233a33b2a7" width="55%"></img>
<img src="https://github.com/Abi-Rai/PortfolioBlazorWasm/assets/52832186/7e972039-d1eb-41c1-8ef9-aec6c5660e37" width="22.1%"></img>

## Credits

- [MudBlazor](https://github.com/MudBlazor/MudBlazor/) : Blazor UI component Library
- [Blazor-ApexCharts](https://github.com/apexcharts/Blazor-ApexCharts) : ApexCharts wrapper for Blazor
- [Pathfinding Visualizer](https://github.com/clementmihailescu/Pathfinding-Visualizer) : Where I got the idea for building the pathfinding page.
