# Packmule 🐴
*A minimal npm-compatible registry built from scratch in .NET 8 — because even tarballs need a mule to haul them.*

---

## Overview
Packmule is a lightweight implementation of the [npm registry API](https://docs.npmjs.com/cli/v9/using-npm/registry).  
It’s designed as a learning project and portfolio piece — showing how to build a functioning npm registry server from scratch, without relying on third-party solutions like Verdaccio or Artifactory.

At its core, Packmule demonstrates that an npm registry is **just**:
- JSON metadata served over HTTP
- Tarball (`.tgz`) files stored and streamed
- A handful of well-defined endpoints

---

## Features
- 📦 Publish packages with `npm publish`  
- ⬇️ Install packages with `npm install`  
- 🎯 Dist-tag support (`npm dist-tag add|rm|ls`)  
- 🗄️ Tarball storage on the local filesystem  
- 🔑 Authentication and object storage backends (planned)

---

## Why the name?
A **pack mule** is sturdy, dependable, and built to carry loads —  
just like this registry is built to carry your npm packages.

---

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)  
- [Node.js](https://nodejs.org/) (to test with npm)  
- Docker (optional, for containerized runs)

### Run locally
```bash
git clone https://github.com/<your-username>/packmule.git
cd packmule
dotnet run
