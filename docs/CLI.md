# SecretSync – CLI Reference

This document describes the command-line interface (CLI) for SecretSync.

The CLI is designed to be:

- Scriptable and automation-friendly.
- Cross-platform (Windows and Linux).

The examples here assume the CLI binary is named:

```
secretsync
```

During development you may run it via:

```
dotnet run --project SecretSync.Cli -- <command> [options]
```

---

## 1. Overview

The CLI supports two main roles:

1. Client machines:
   - Enroll with a SecretSync server.
   - Fetch and cache project secrets.
   - Keep local versions in sync with server versions.

2. Owner (server operator):
   - Approve or revoke clients.
   - Manage projects and secrets.

The CLI uses a local configuration file to store:

- ~/.secretsync/config.json:
  - Server URL
  - Client ID
  - Token (once approved)
  - Some general settings options (e.g: logging)
- ~/.secretsync/versions.json
  - Per-project local versions

---

## 2. Configuration

### 2.1 Default Config Location

By default, the CLI reads and writes its configuration from a file such as:

- Linux/macOS: `~/.secretsync/config.json`
- Windows: `%USERPROFILE%\.secretsync\config.json`

### 2.2 Configuration Fields

Example config:

```json
{
  "serverUrl": "https://localhost:5001",
  "clientId": "e8c255bd-5f63-4b09-8f0b-2ac35f3fbaef",
  "token": "stored-client-token",
  "options": {
    logging: false,
    polling: "automatic",
    polling-interval: "5" # In minutes
  }
}
```

Example versions:
```json
"projects": {
  "Netcode": {
    "projectId": "6d9fc7b7-1111-4e72-9e52-8d927d53abcd",
    "localVersion": 4
  }
}
```

---

## 3. Global Options

Global options (proposed):

- `--config <path>` # If you want to make this read from a seperate config file
- `--server <url>` # Tell it what server to use, useful if using multiple
- `--json` # Force json output in case of using with stuff needing to parse commands results
- `-v`, `--verbose` # Show debugging information

---

## 4. Client Commands

### 4.1 enroll

```
secretsync enroll --name <machine-name> [--server <url>]
```

Registers the current machine as a client with the server.

### 4.2 status

```
secretsync status
```

Checks the current enrollment status.

### 4.3 list projects

```
secretsync list projects
```

Shows project IDs, names, and versions.

### 4.4 list secrets

```
secretsync list secrets <project-name-or-id>
```

Lists secret keys (not values) for a project.

### 4.5 get

```
secretsync get <project-name-or-id> <key>
```

Fetch a decrypted secret.

### 4.6 pull

```
secretsync pull <project-name-or-id>
```

Synchronizes local secrets using project versioning.

### 4.7 export

```
secretsync export <project> --out <path> [--format <format>]
```

Exports secrets to .env or JSON.

---

## 5. Owner Commands

### 5.1 owner list clients

```
secretsync owner list clients
```

### 5.2 owner approve

```
secretsync owner approve <client-id>
```

### 5.3 owner revoke

```
secretsync owner revoke <client-id>
```

### 5.4 owner create project

```
secretsync owner create project <name>
```

### 5.5 owner set secret

```
secretsync owner set secret <project> <key> <value>
```

---

## 6. Exit Codes

- `0` success  
- `1` general error  # Catch all
- `2` validation error # Used when the user themselves inputs the command incorrectly
- `3` auth error # If Authentication token is invalid
- `4` not found  # Secret / Project not found and/or HTTP 404 response

---

## 7. Examples

### First-time setup

```
secretsync enroll --name MyLaptop --server https://secretsync.local
secretsync status
secretsync list projects
secretsync pull Netcode
secretsync get Netcode DB_PASSWORD
```

### Owner flow

```
secretsync owner list clients
secretsync owner approve e8c255bd-5f63-4b09-8f0b-2ac35f3fbaef
secretsync owner approve Jeffs-laptop
secretsync owner create project Netcode
secretsync owner set secret Netcode DB_PASSWORD "super-secret"
```

---

## 8. Future CLI Extensions

Possible additions include:

- Secret deletion
- Project deletion
- Synchronizing of local secret with git and with existing .env and other secrets
- Allow for admin roles that can specific perms for specific projects and/or secrets to allow for easy collaboration