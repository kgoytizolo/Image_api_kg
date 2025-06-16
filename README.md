# Image_api_kg
A .NET Core API solution for image uploading / downloading


# Project Overview
**Image_api_kg** is a cross-platform system, which delivers API endpoints to support image uploading in a flexible and simple way. It was designed to provide a better image manipulation by resizing several types of pictures, without losing initial image's proportions. This application offers predefined resolutions, which are based on the most common image standards used on the internet, as well as a customized resolutions defined by the user. 


# Technology Stack
**Image_api_kg** is based on .NET Core 9 and C# 13. It uses the following frameworks:

**1. For database manipulation and ORM:** Entity Framework Core, SqLite EF Core Database Provider. 

**2. CQRS Validation Pipeline:** MediatR, FluentValidation.

**3. Logging:** Serilog (to apply configuration in AppSettings, physical files and console).

**4. API endpoint's documentation:** Swagger.

**5. Image manipulation:** ImageSharp. 


# Software Architecture and Project Structure
This application follows a layered clean architecture for an ASP.NET Core solution, targeting .NET 9. Here’s a concise summary of its structure:

**•	Presentation Layer (Web API):**
The ImageWebApi project exposes RESTful endpoints using ASP.NET Core controllers (ImagesController). It handles HTTP requests, input validation, and response formatting.
Controllers depend on abstractions (like IMediator) and do not contain business logic.

**•	Application Layer:**
This layer contains application logic, including commands and queries (e.g., UploadImageCommand, ResizeImageCommand, GetImageQuery). It uses the Mediator pattern (via MediatR) to decouple request handling from controllers. This layer orchestrates domain operations and coordinates between the presentation and domain layers.

**•	Domain Layer:**
The domain layer (e.g., Domain.Images.Image, PredefinedImages) encapsulates core business entities, rules, and logic. It is independent of infrastructure and application concerns, focusing solely on business rules and invariants.

**•	Infrastructure Layer:**
This layer provides implementations for technical concerns such as data access, file storage, and third-party integrations. It is registered in the DI container via extension methods (e.g., AddInfrastructureLayer).

**•	Shared Project:**
The Shared project contains cross-cutting concerns and utilities, such as image processing (using SixLabors.ImageSharp) and shared DTOs or constants.

**•	Cross-Cutting Concerns:**
Logging (via Serilog), validation, and error handling are integrated throughout the application, ensuring observability and robustness.

**Key Patterns and Technologies:**

•	MediatR for CQRS and decoupling.

•	Dependency Injection for managing service lifetimes.

•	Serilog for structured logging.

•	SixLabors.ImageSharp for image processing.

•	xUnit & Moq for unit testing.

This architecture promotes separation of concerns, testability, and maintainability, making it suitable for scalable and robust web APIs.



# Getting Started

**1. Prerequisites:**

**1.1 NET 9 SDK and Runtime**

Make sure that you have installed .NET 9 SDK (it can work even with the latest SDK 9.0.300):
```bash
dotnet --list-sdks
```
•	You must have the .NET 9 SDK installed on your development and build machines.

•	The .NET 9 runtime must be available on any machine where the application will run.

**1.2 Development Tools**

•	Visual Studio 2022 (with .NET 9 support) or Visual Studio Code with the C# extension.

•	.NET CLI (dotnet command) for building, running, and managing the project.

**1.3 Docker**

Make sure that you have the latest version of Docker in order to run this application through containers:
```bash
docker --version
```

**1.4 SQLite**

Make sure that you have the latest SQLite3 version in order to use the attached database properly.
 
**2. Clone the repository**

To clone **Image_api_kg** locally, just run the following git command:
```bash
git clone https://github.com/kgoytizolo/Image_api_kg.git
```

# Running the API Locally

1. Quickstart Using Kestrel Web Server and HTTPS:

- Just execute F5 for Debugging or CTRL+F5 to run the application locally. Make sure that it points to > https and ImageWebApi is setted as the Startup Project (it currently does).

- This application points to Development by default. Corresponding SQLite database is stored in the same path as the Web API project and already has Images and Users tables created.

- It should redirect to the following url:

**https://localhost:7136/swagger/index.html**

3. Using Docker:

# API Documentation

This application has implemented Swagger in order to keep an updated documentation of its API endpoints so they can be tested locally and referenced by any client who would use any of this services:

- Go to the following url (launched by default once the project is running locally) to look for documentation and local testing:

https://localhost:7136/swagger/index.html

# Database & Migrations
Database technology and how to apply migrations

