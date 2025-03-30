# Ansible

Ansible is a Zettelkasten note-taking service that allows you to create, organize, and link notes in a powerful knowledge graph. Part of the Cosmos microservices system.

## Project Structure

```
Ansible/
├── src/                         # Source code
│   ├── Ansible.API/             # API controllers and endpoints
│   ├── Ansible.Domain/          # Domain models and interfaces
│   ├── Ansible.Infrastructure/  # Implementation of domain interfaces
│   └── Ansible.Host/            # Application host and entry point
├── tests/                       # Test projects
│   ├── Ansible.UnitTests/       # Unit tests
│   └── Ansible.IntegrationTests/# Integration tests
├── infrastructure/              # Docker and Kubernetes configurations
└── Ansible.sln                  # Solution file
```

## Getting Started

### Prerequisites

- .NET 9.0 SDK
- Docker and Docker Compose (for containerized development)

### Development

1. Clone the repository

2. Build the solution
```
dotnet build
```

3. Run the API
```
dotnet run --project src/Ansible.Host/Ansible.Host.csproj
```

### Using Docker

1. Build and run using Docker Compose
```
cd infrastructure
docker-compose --profile dev up --build
```

## Features

- Create and organize notes with tags
- Link notes together to form a knowledge graph
- Search notes by content or tags
- Integrate with Azure DevOps work items
- RESTful API for easy integration with other systems

## API Endpoints

### Notes
- `GET /api/notes` - Get all notes
- `GET /api/notes/{id}` - Get a specific note
- `POST /api/notes` - Create a new note
- `PUT /api/notes/{id}` - Update a note
- `DELETE /api/notes/{id}` - Delete a note
- `GET /api/notes/search?term={searchTerm}` - Search notes
- `GET /api/notes/bytag/{tagName}` - Get notes by tag
- `GET /api/notes/{id}/linked` - Get notes linked to a specific note
- `POST /api/notes/link` - Create a link between notes
- `DELETE /api/notes/link/{linkId}` - Delete a link

### Tags
- `GET /api/tags` - Get all tags
- `GET /api/tags/{id}` - Get a specific tag
- `POST /api/tags` - Create a new tag
- `PUT /api/tags/{id}` - Update a tag
- `DELETE /api/tags/{id}` - Delete a tag

## Integration with Spaceport API

Ansible connects to the Spaceport API to fetch Azure DevOps work items for associating notes with work contexts.