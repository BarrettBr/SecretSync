## What are these?

- These are all "Mermaid" diagrams/files, a way to easily make diagrams using text for easily replicating.
- [Link to render mermaid files](https://mermaid.live)

### High-level overview:

- SystemContext: Outside world view
- Container: High-level internals
- Component: Detailed internals
- ServerDiagram: Deployment & where things run
- Enroll / Polling: Behavior & general flow

### File Description:

- SystemContext: Who talks to SecretSync.Server
- ContainerDiagram: High level "client/server/db" and server subcontainers
- ComponentDiagram: Goes inside of the containers and shows
  - API Endpoints
  - Services
  - Interfaces
  - Store + Encrypt infrastructure
- ServerDiagram: Deployment diagram, gives high level deployment overview
- Enroll_SequenceDiagram: Sequence flow for enrollment, approval, and polling for authentication token
- Polling_SequenceDiagram: Sequence flow for version polling, fetching updated secrets and decrypting of secrets