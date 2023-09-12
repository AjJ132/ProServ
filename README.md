# Table of Contents
1. [Introduction](#introduction)
2. [Features](#features)
3. [Technologies Used](#technologies-used)
4. [Prerequisites](#prerequisites)
5. [Installation](#installation)
6. [Usage](#usage)

## Introduction
Welcome to ProServ, an evolving communication platform designed to bridge the gap between coaches and athletes. This application, although a work in progress, aims to streamline the distribution of workouts and improve overall communication within athletic teams.

As a student software engineer, this project allows me to dive into various aspects of full-stack development, focusing primarily on C# technologies like Blazor for the frontend and ASP.NET for the backend. The accompanying mobile iOS app brings added versatility to the platform.

## Features
- User Login
- Workout distribution
- Mobile app
- Secure backend API endpoints
- Fully deployable in Azure with CI/CD pipeline

## Technologies Used
- Azure SQL Database
- Github Actions
- CI/CD Pipeline
- C#
- Blazor with Radzen UI libraries
- SwiftUI 

## Installation

### Prerequisites
For the time being, this application is not installable due to special database access requirements.

**Note:** This project is a work in progress as of September 12, 2023. Some features may still be under development.

#### Frontend: Blazor Application
1. Clone the repository to your local machine
2. Navigate to the frontend directory
   ```bash
   cd ProServ/Client 
   ```
3. Restore the packages
   ```bash
   dotnet restore 
   ```
4. Run the application
   ```bash
   dotnet run 
   ```

#### Backend: ASP.NET
1. Navigate to the backend directory
   ```bash
   cd ProServ/Server 
   ```
2. Restore the packages
   ```bash
   dotnet restore 
   ```
3. Run the application
   ```bash
   dotnet run 
   ```

#### Mobile iOS App
1. Open the project in Xcode
2. Select your preferred simulator or connect your iOS device
3. Build and run the application

## Usage
To run the system locally, it's important to start the server application first, as it serves as the API server for both the client application and the mobile app. Follow these steps to ensure a smooth setup:
1. Start the Server Application: Begin by launching the server application to initialize the API server.
2. Run the Client Application: After the server is up and running, you can proceed to launch the client application.
3. Mobile App: Alternatively, or in addition, you can also run the mobile app.

### Note on Ports and Addresses:
- The server application should be running on `localhost:5001` to correctly handle API requests.
- When running locally, the system will automatically switch to developer mode and use the `localhost:5001` address instead of the Azure online address.
