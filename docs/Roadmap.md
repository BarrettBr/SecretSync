# SecretSync – Project Roadmap
## Fair Warning:
- Everything listed in this document might change, this was mainly written to put thoughts in order not a nessecary must do in order list. This means this might be deleted/revamped at any time.

---

# **Phase 0 – Project Setup & Skeleton**
_**Goal:** Establish structure, layer boundaries, and dependency flow._

### Deliverables
- Solution structure created:
  - `SecretSync.Cli` (Command-line client)
  - `SecretSync.Server` (Transport layer)
  - `SecretSync.Application` (Application/Service layer)
  - `SecretSync.Core` (Domain models + interfaces)
  - `SecretSync.Infrastructure` (Database + encryption)
- Unit testing project created (`SecretSync.Tests`)

### Success Criteria
- Compiles and runs across Windows and Linux.
- Clear separation of concerns

---

# **Phase 1 – Domain & Storage Foundation**
_**Goal:** Implement core domain models + basic database layer._

### Deliverables
- Domain Components:
  - `Project`, `Secret`, `Client`, `AuditLogEntry`
- Domain rules:
  - Project versioning
  - Client status workflow (Pending → Approved → Revoked)
- Storage interfaces defined:
  - `IProjectStore`
  - `ISecretStore`
  - `IClientStore`
  - `IAuditStore`
- SQLite implementation (Infrastructure)

### Success Criteria
- Infrastructure / Database Layer layer fully functional.
- Application layer can call repositories through interfaces.

---

# **Phase 2 – Encryption & Key Management**
_**Goal:** Implement secure secret storage using modern encryption (AES)._

### Deliverables
- `IEncryptionProvider` interface defined
- AES encryption/decryption implementation
- Argon master-key generation
- Master key loading strategy (e.g., password prompt, config file, environment variable)

### Success Criteria
- Raw database/file storage contains only encrypted values.
- Secrets decrypt correctly when requested.
- Crypto implementation covered by unit tests.

---

# **Phase 3 – Client Enrollment & Authentication**
_**Goal:** Enroll client and pass out tokens._

### Deliverables
- `POST /enroll` endpoint
- Pending → Approved workflow
- Owner-issued client tokens
- Allow owner to generate/add token and then be able to pass it out manually
- Token hashing for DB
- Authentication middleware
- Authorization rules (project-level access)

### Success Criteria
- Only approved clients can access secrets.
- Tokens persisted and verified securely.
- CLI can enroll and reauthenticate automatically.

---

# **Phase 4 – Project & Secret Management**
_**Goal:** Expose CRUD operations for owners; versioning system online._

### Deliverables
- Owner endpoints:
  - Create project
  - Add/update/delete secret
  - Bump project version automatically
- Client read endpoints:
  - List projects the client has access to
  - Get project version (`GET /projects/{id}/version`)
  - Read secrets (`/secrets` and `/secret/{key}`)
  - Support `?sinceVersion=X` filtering

### Success Criteria
- Version-based polling fully operational.
- Clients stay up to date on secret changes.
- Secret updates securely logged in AuditLog.

---

# **Phase 5 – Logging and Metrics**
_**Goal:** Add logging and general metrics._

### Deliverables
- General logging of events & errors
- Metrics endpoint:
  - `secrets_read_total`
  - `secrets_updated_total`
  - `client_enrollments_total`
  - Request latency
- Audit log stored in DB for:
  - Secret read
  - Secret update
  - Client enrollment

### Success Criteria
- Each project action generates auditable entries.
- Metrics visible in Prometheus-style scraping.
- Logging supports debugging + long-term analysis.

---

# **Phase 6 – CLI Client Implementation**
_**Goal:** Deliver a user-friendly command-line tool._

### Deliverables
- Cross-platform CLI (`SecretSync.Cli`)
- Commands:
  - `secretsync enroll`
  - `secretsync approve <clientId>` (owner)
  - `secretsync get <project> <key>`
  - `secretsync list projects`
  - `secretsync list secrets <project>`
  - `secretsync pull <project>`
  - `secretsync export <project>`
  - `secretsync guesttoken`
	- This allows for registering / printing randomly generated token
  - `secretsync register <token>` 
- Automatic token storage (local config file)
- Local caching of secrets + versions

### Success Criteria
- Entire workflow usable via CLI.
- Clients can stay synced using polling.
- CLI supports simple, intuitive UX.

---

# **Phase 7 – GUI Client (Optional)**
_**Goal:** Build a user-friendly graphical manager for owners._

### Deliverables
- GUI built using Avalonia (cross-platform) or another GUI
- Views:
  - Client & Owner: 
    - Projects list
    - Secrets table
  - Owner Only:
    - Client approvals
    - Log viewer & Metric Visualization
- Connects exclusively through the REST API

### Success Criteria
- Owners can manage secrets visually.
- GUI adds convenience but is fully optional.

---

# **Phase 8 – Advanced Features (Future)**
_**Goal:** Expand capabilities for power users and long-term value._

### Possible Features
- gRPC transport
- Github integration
- Webhooks / Notifications
  - Notify clients when secrets rotate
- Secret Templates
  - GitHub repo scanner for `.env`, config files
- Secret expiration + rotation policies
- Backup + restore tooling
- Import from .env, JSON, YAML

---

# **Phase 9 – Hardening & Deployment**
_**Goal:** Production stability & distribution._

### Deliverables
- Dockerfile for server
- CI/CD pipeline (GitHub Actions)
- Performance testing (High load tests)
- Security Hardening:
  - Brute-force protections
    - Rate limiting polls, etc
  - General protections for keys/rotation

### Success Criteria
- Server runs in Docker and bare-metal.
- Self-contained binaries for all platforms.

---

# **End Goal**

SecretSync becomes a polished, secure, cross-platform secret manager with:

- clean architecture  
- strong encryption  
- versioned sync model  
- a great CLI  
- optional GUI