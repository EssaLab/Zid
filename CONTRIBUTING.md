# Contributing to EssaLab.Zid 🤝

Thank you for considering contributing to **EssaLab.Zid**! We welcome contributions from developers of all skill levels.

---

## 🚀 How Can I Contribute?

### 1. Reporting Bugs
Before creating a bug report, please check existing issues to avoid duplicates. When reporting a bug, include:
- Clear steps to reproduce the issue.
- Expected vs actual behavior.
- Your .NET runtime version and OS details.

### 2. Suggesting Features
Enhancement suggestions are tracked as GitHub issues. Please open an issue describing your proposal and use case.

### 3. Pull Requests
1. Fork the repository and create your branch from `main`:
   ```bash
   git checkout -b feature/my-amazing-feature
   ```
2. Ensure your code follows .NET coding standards and idioms.
3. Write/update tests for your changes.
4. Run all benchmarks if modifying critical paths (`Zid.Next()` or formatting methods).
5. Open a Pull Request referencing the related issue.

---

## 🧪 Development & Testing

Build the solution and run tests locally:

```bash
dotnet build
dotnet test
```

To run performance benchmarks:

```bash
cd src/EssaLab.Zid.Benchmarks
dotnet run -c Release
```

---

## 📜 Code of Conduct

Please be respectful and constructive in all community interactions.