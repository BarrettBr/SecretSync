# SecretSync – API Specification

This document defines the REST API used by SecretSync clients and the server.  

All endpoints return a JSON object.  
All client-authenticated endpoints require:

```
Authorization: Bearer <token>
```

---

# 1. Enrollment API

Clients must enroll before receiving secrets.  
The owner must (currently) manually approve them.

## 1.1 POST /enroll

Registers a new client.

**Request**
```json
{
  "clientId": "GUID",
  "name": "My-Computer"
}
```

**Response**
```json
{
  "status": "Pending"
}
```

If the client already exists, the server reuses the existing record otherwise it will return "Pending" to tell the application work is being done. This is done in this 2-step approach to help allow for seperation of issues and how we handle that.

---

## 1.2 GET /enroll/status/{clientId}

Checks whether the client has been approved.

**Responses**

Pending:
```json
{
  "status": "Pending"
}
```

Approved:
```json
{
  "status": "Approved",
  "token": "<client-auth-token>"
}
```

Revoked *mainly for rate limiting and general request refusal*:
```json
{
  "status": "Revoked"
}
```

---

# 2. Owner Endpoints

Owner endpoints require owner authentication (e.g., local password or environment token).  
These are not accessible by regular clients.

## 2.1 POST /projects

Creates a new project.

**Request**
```json
{
  "name": "Netcode"
}
```

**Response**
```json
{
  "projectId": "GUID",
  "name": "Netcode",
  "version": 0
}
```

---

## 2.2 POST /projects/{projectId}/secrets

Creates or updates a secret.  
Updating a secret increments the project version.

**Request**
```json
{
  "key": "DB_PASSWORD",
  "value": "plaintext-or-env-value"
}
```

**Response**
```json
{
  "projectId": "GUID",
  "key": "DB_PASSWORD",
  "updated": true,
  "newVersion": 4
}
```

---

## 2.3 POST /clients/{clientId}/approve

Approves a pending client and issues a token.

**Response**
```json
{
  "clientId": "GUID",
  "status": "Approved",
  "token": "<client-auth-token>"
}
```

---

## 2.4 POST /clients/{clientId}/revoke

Revokes a client’s access.

**Response**
```json
{
  "clientId": "GUID",
  "status": "Revoked"
}
```

---

# 3. Client Read API

These endpoints require a valid client token.

## 3.1 GET /projects

Lists all projects the client has access to.

**Response**
```json
[
  {
    "projectId": "GUID",
    "name": "Netcode",
    "version": 4
  },
  {
    "projectId": "GUID",
    "name": "Cool-Game",
    "version": 7
  }
]
```

---

## 3.2 GET /projects/{projectId}/version

Returns the current version of a project.

**Response**
```json
{
  "version": 5
}
```

Used for polling.

---

## 3.3 GET /projects/{projectId}/secrets?sinceVersion=X

Returns secrets that changed since a specific version.

**Response**
```json
{
  "version": 5,
  "secrets": [
    { "key": "DB_PASSWORD", "value": "decrypted-value" },
    { "key": "API_URL", "value": "https://example.com" }
  ]
}
```

If no secrets changed:
```json
{
  "version": 5,
  "secrets": []
}
```

---

## 3.4 GET /projects/{projectId}/secret/{key}

Fetches an individual secret.

**Response**
```json
{
  "key": "DB_PASSWORD",
  "value": "decrypted-value",
  "version": 5
}
```

---

# 4. Logging API (Optional)

If logging is enabled, the owner can retrieve logs.  
Limit is there if you want to only recieve X recent ones

## 4.1 GET /audit/logs?projectId=&clientId=&limit=

Example:
```
GET /audit/logs?projectId=GUID&limit=50
```

**Response**
```json
[
  {
    "id": "GUID",
    "timestamp": "2025-01-01T12:30:00Z",
    "clientId": "GUID",
    "projectId": "GUID",
    "operation": "ReadSecret",
    "secretKey": "DB_PASSWORD"
  }
]
```

---

# 5. Error Handling

All errors are returned in a consistent format.

**Example**
```json
{
  "error": "Unauthorized",
  "message": "Client token is invalid or expired."
}
```

Common status codes:
- 200 Ok
- 400 Bad Request
- 401 Unauthorized
- 403 Forbidden
- 404 Not Found
- 409 Conflict (Used for versioning)
- 500 Internal Server Error

---

# 6. Security Notes

- All secret values are returned decrypted to approved clients.
- Secrets are stored encrypted using AES encryption.
- Client tokens must be sent in the Authorization header.

---

# 7. Future Extensions

- gRPC transport
- Allow owner to manage sub-groups so as to allow for admins to be able to allow on their behalf
- Allow automatic scanning of github repos and allowing for linking of both existing projects + new projects to github repos to allow for automatic scanning & detection of existing secrets
- Webhooks for automatic secret rotation
- Secret expiration policies