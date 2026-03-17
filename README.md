# Image Layout Composer

A full-stack solution built with ASP.NET Core Web API and Blazor WebAssembly to compose images into various grid layouts.

## Features

- **Patient-Centric Upload**: Enter a patient name and upload multiple images (JPG/PNG).
- **Search & Filter**: Find images specifically for a patient before composing.
- **Grid Layouts**: Support for 2x2, 3x3, and 4x4 grids.
- **Enterprise Ready**: Includes global exception handling, Serilog logging, and Correlation ID tracking.
- **Smart Resizing**: Automatically fits images into grid cells with 10px padding while preserving aspect ratio.

## Project Structure

- `src/ImageLayoutComposer.Api`: ASP.NET Core Web API handling storage and image processing.
- `src/ImageLayoutComposer.Client`: Blazor WebAssembly frontend.
- `src/ImageLayoutComposer.Shared`: Common models, DTOs, and constants.
- `tests/ImageLayoutComposer.Tests`: Unit tests for core logic.

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later.
- [Git](https://git-scm.com/downloads)

## Setup and Run

### 1. Clone the Repository
```bash
git clone https://github.com/LarryBui/ImageComposer.git
cd ImageComposer
```

### 2. Trust HTTPS Development Certificates
Required for secure local communication between the Blazor client and the API:
```bash
dotnet dev-certs https --trust
```

### 3. Build the Solution
```bash
dotnet build
```

### 4. Run the Application

You must run both the API and the Client.

#### **Run the API (Backend)**
Open a terminal:
```bash
cd src/ImageLayoutComposer.Api
dotnet run --launch-profile https
```
The API starts at `https://localhost:7120`. Interactive docs are available at `https://localhost:7120/swagger`.

#### **Run the Client (Frontend)**
Open a second terminal:
```bash
cd src/ImageLayoutComposer.Client
dotnet run
```
The application will be available at the URL shown in the output (e.g., `https://localhost:7187`).

## API Usage & Testing

- **Swagger UI**: Access `https://localhost:7120/swagger` to test endpoints manually.
- **Error Handling**: Test the global exception handler by visiting `https://localhost:7120/api/images/test-error`. You should receive a clean JSON error response with a Correlation ID.

## Assumptions and Tradeoffs

- **Storage**: Images are stored in `src/ImageLayoutComposer.Api/uploads/`. Metadata is in-memory (lost on restart).
- **Security**: Filenames are stored as `{PatientName}_{GUID}{Extension}` to prevent path traversal and collisions.
- **Validation**: Strict file extension (.jpg, .jpeg, .png) and size (10MB) limits are enforced on both client and server.

## Troubleshooting

- **CORS/Connection**: If the client cannot find the API, ensure the API is running on port **7120**. This is configured in `src/ImageLayoutComposer.Client/wwwroot/appsettings.json`.