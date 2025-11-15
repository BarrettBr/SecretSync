# SecretSync
A Secure, Cross-Platform Secret Manager for Small Teams

SecretSync is a lightweight secret manager designed for developers and small teams that want a simple but secure way to share secrets across multiple machines.

It offers an owner-to-client model where one trusted machine hosts encrypted projects and approved clients can sync secrets securely on demand.

SecretSync is built with strong encryption and a backend-first design.

---
## Installation
### Requirements
- .Net 9 or newer
- SQLite
- Docker optional for certain release packages

### Starting Project
- Clone the repo: `git clone <git_url>`
- dotnet build
- dotnet run


---
## Features

### Security
- Secrets are encrypted using AES encryption.
- Master key is derived using Argon.
- Uses HTTPS for encrypted transport.
- Per-client authentication tokens.
- Per-project authorization.

### Syncing and Versioning
- Multi-project secret management.
- Automatic version increments when secrets update.
- Clients fetch only modified data using:
  - GET /projects/{id}/version
  - GET /projects/{id}/secrets?sinceVersion=X
- Local client caching.

### Cross-Platform
- Runs on Windows, Linux, and macOS.
- CLI-first design, with optional GUI planned.

### Clean Architecture
SecretSync is organized into clear layers for more information view docs/Architecture.md:
- Transport (Server): REST API and routing
- Application: Workflows such as enroll/approve/get/update
- Domain: Models and Rule Handling
- Infrastructure: SQLite, AES encryption, and general logging / metric information

### Logging & Metrics
- Allows for optional audit logs for read and write operations so as to allow for minimal peformance hits if wanted.
- Detailed Logging reports
- Metrics endpoints and visualizations.

---

## Use Cases

- Share secrets with collaborators.
- Sync environment variables across multiple devices.
- Securely distribute API keys, DB connection strings, and/or tokens.
- Manage secrets per project.
- Create or export local backups.

---
## Basic Workflow Example:
1. Client Enrolls:
```
secretsync enroll --name MyLaptop
```
2. Owner Approves
```
secretsync approve <clientId>
```
3. Client Fetches secrets
```
secretsync pull <project>
```
4. Owner updates secret
```
secretsync set <project> API_KEY "new-value"
```
5. Afterwards the client will detect the change via automatic or manual polling

---
## Testing:
- To Run the tests run: `dotnet test`
- This is built off of the xUnit testing framework