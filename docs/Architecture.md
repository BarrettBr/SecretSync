# SecretSync – Architecture Overview

## 1. Goals

SecretSync is a self-hosted secret manager designed for small teams.

**Primary goals:**

- Simple way to share **many secrets** across multiple machines.
- **Owner-server** model: one user hosts the secrets; others connect as clients.
- **Multi-project** support: one owner managing secrets for several projects.
- Works on **Windows and Linux** via .NET framework.
- Both **CLI** and **GUI** interaction (GUI is currently optional and can come later in the design process).
- Includes functionality to export secrets to a file for ease of backing up
- Backend-focused design emphasizing:
  - Clean architecture
  - Security & Encryption
  - Logging & Metrics
  - Tested and Extensible

---

## 2. Core Use Cases

1. **Owner creates a project and secrets**
   - Owner defines a project (e.g., `Cool-Games`, `Netcode`, etc).
   - Adds secrets like `DB_PASSWORD`, `API_KEY`, etc.
   - Would like eventually automatic adding of github repos if git added

2. **Client enrolls & gets approved**
   - Client enrolls with the server.
   - Owner approves the client.
   - Client receives credentials (token) for future calls.
	   - The token will be automatically saved to a local file for future requests

3. **Client fetches secrets**
   - Automatic polling is optional
   - Client polls for project version.
   - If local version is out of date, client fetches updated secrets.
   - Client gets secrets via CLI commands or HTTP calls.

4. **Owner rotates / updates secrets**
   - Owner updates one or more secrets in a project.
   - Project version increments.
   - Clients detect version change on next poll and update.

5. **Auditing**
   - Allow for turning off for performance/space
   - Server logs access events:
     - which client accessed which project/secret
     - when
   - Potentially do trace logs of critical events i.e: database accesses/deletions

---

## 3. High-Level Architecture

SecretSync is split into layers that each have their own responsibilty: <br>
More information: [Microsofts common architectures](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures)

### Frontend:
- **Client Layer (CLI, GUI, language specific)**
  - **CLI (`SecretSync.Cli`)** used by humans or scripts.
  - (Future) **GUI (`SecretSync.Gui`)** for easier owner management.
  - Other languages (Python, GoLang, etc) will interact via REST API requests

### Middle-end:
- **Server Layer (Transport Layer; `SecretSync.Server`)**
  - Exposes APIs via **HTTP/JSON REST**.
  - (Optional) support for **gRPC**
  - Handles authentication middleware, authorization, and routing to services

### Backend:
- **Application / Service Layer**
  - Implements use-cases & workflows (EnrollClient(), GetSecret(), UpdateSecret() etc.)
  - Think of this more like "if given X how do I do Y"
  - Coordinates domain logic and storage/encryption components.
  - Enforces rules like version bumps and audit logging

- **Core / Domain Layer (`SecretSync.Core`)**
  - Contains models (Project, Secret, Client, AuditLog).
  - Encodes domain rules (e.g., access checks, versioning).
  - Defines interfaces for storage, encryption, and logging.
  - No infrastructure, rules + invariants, self-contained logic
	  - "If given these set of inputs do task on what I have"
  - Doesn't directly interface with upper layers and stuff so for example if needing to interact with a websocket pass that information to this not the item itself.

- **Infrastructure / Database Layer**
  - This is the "Where & How layer of storing" doing the technical work of storing stuff in the database/logging
  - Stores data using LiteDB (non-relational) or SQLite (relational).
  - Implements interfaces: `ISecretStore`, `IClientStore`, `IProjectStore`, and `IAuditStore`.
  - Provides encryption (using things like AES).

### Multiple layer functionality:
- **Security & Encryption**
  - Secrets encrypted using symmetric encryption like AES.
  - Master key derived from an owner passphrase (Argon2).
  - Per-client authentication tokens and per-project authorization.

- **Logging & Metrics**
  - Logging
  - Metrics endpoint for requests, errors, and latency & visualization.
	  - Heard of things like [Prometheus](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/metrics-collection) that might be good for this
  - Optional tracing via OpenTelemetry or other frameworks.

---

## 4. Components

### 4.1 Core Components

- **Project**
  - Description: Used to hold a project itself
  - `Id: Guid`
  - `Name: string`
  - `Version: long`
  - `OwnerId: string`
  - Collection of `Secrets`

- **Secret**
  - Description: Used to hold one secret
  - `Id: Guid`
  - `ProjectId: Guid`
  - `Key: string`
  - `EncryptedValue: byte[]`
  - `CreatedAt: DateTime`
  - `UpdatedAt: DateTime`

- **Client**
  - Description: Used to hold a client that is connected as well as their token
  - `Id: Guid`
  - `Name: string` (e.g., "My-Computer", used as frontend shorthand)
  - `Status: Pending | Approved | Revoked`
  - `AuthTokenHash: string`
  - `CreatedAt: DateTime`
  - `LastSeenAt: DateTime?`

- **AuditLogEntry**
  - Description: Used to define & hold a log
  - `Id: Guid`
  - `Timestamp: DateTime`
  - `ClientId: Guid`
  - `ProjectId: Guid`
  - `Operation: enum { ReadSecret, ListSecrets, UpdateSecret, CreateProject }`
  - `SecretKey: string?` (optionally hashed)

---

## 5. API Design

### 5.1 Authentication

- Clients authenticate using a bearer token:
  - `Authorization: Bearer <token>`
- Tokens issued once the owner approves the request.

### 5.2 API Endpoints

**Enrollment**

- Use the Pending -> GET status bit to validate that the enrollment went through and worked
- `POST /enroll`
  - Request: `{ "clientId": "GUID", "name": "My-Computer" }`
  - Response: `{ "status": "Pending" }`
- `GET /enroll/status/{clientId}`
  - Response:
    - Pending: `{ "status": "Pending" }`
    - Approved: `{ "status": "Approved", "token": "<token>" }`

**Owner Operations**

- `POST /projects`
  - Create a new project.
- `POST /projects/{projectId}/secrets`
  - Create/update a secret.
  - On update, project version increments.
- `POST /clients/{clientId}/approve`
  - Approve a client and issue a token.

**Client Read Operations**

- `GET /projects`
  - List projects the client has access to.
- `GET /projects/{projectId}/version`
  - Returns `{ "version": 5 }`
  - Used in polling
- `GET /projects/{projectId}/secrets?sinceVersion=3`
  - Returns:
	 ```json
	{
	  "version": 5,
	  "secrets": [
	    { "key": "DB_PASSWORD", "value": "..." },
	    { "key": "API_URL", "value": "..." }
	  ]
	}
	 ```
- `GET /projects/{projectId}/secret/{key}`
  - Returns current value and project version.

---

## 6. Versioning & Polling

Each project has an increasing `Version` value.

- On any secret create/update/delete:
  - `Project.Version++`
- Clients track `LocalVersion` per project.

**Polling flow:**

1. Client calls `GET /projects/{projectId}/version`.
2. If `ServerVersion == LocalVersion`:
   - No updates; client does nothing.
3. If `ServerVersion > LocalVersion`:
   - Client calls `GET /projects/{id}/secrets?sinceVersion=LocalVersion`.
   - Updates local cache and sets `LocalVersion = ServerVersion`.

---

## 7. Security Model

- **Encryption at Rest**:
  - Secret values are encrypted before being stored.
  - Master key derived from a passphrase via hashing functions (e.g. ARGON2).
- **Ensuring Encryption in Transit**:
  - Served over HTTPS to ensure data transfered securely.
- **Client Authentication**:
  - New clients must enroll.
  - Owner explicitly approves clients.
  - Approved clients receive tokens for authenticated calls. These are sent in future requests
- **Authorization**:
  - Per-project access control, only allowing authorized clients to read secrets of given projects.
- **Audit Logging**:
  - All sensitive operations (reads/updates) logged with client ID, project, and timestamp.

---

## 8. Logging & Metrics

- **Logging**:
  - Logs for general Info & Errors.
  - Includes client ID, project, and operation.
- **Metrics**:
  - Counters:
    - `secrets_read_total`
    - `secrets_updated_total`
    - `client_enrollments_total`
  - Histograms:
    - request latency per endpoint
- **Tracing (optional)**:
  - Use OpenTelemetry to trace individual requests.
  - Spans for important operations like decryption and database access.

---

## 9. Extensibility

SecretSync is designed with interfaces and dependency inversion to make it extensible:

- Swap storage implementation (file, SQLite, LiteDB, PostgreSQL).
- Add additional transports like gRPC.
- Add more authentication strategies (e.g., mutual TLS, OAuth for enterprise use, etc)
- Future UI clients (web, desktop, or mobile) all talk to the same API.