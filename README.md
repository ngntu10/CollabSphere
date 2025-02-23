<!-- markdownlint-disable MD032 MD033-->

# 🔥 **N-TIER SINGLE APPLICATION BACKEND TEMPLATE**

---

## 🤔 **What is this project?**

- This is the N-tier single application backend template.

---

## 📚 **What does it include?**

---

### 🌲 **Project tree**

```
NTierArchitecture
    ├── Common
    ├── Configs
    ├── Database
    │   └── Migrations
    ├── Entities
    │   ├── Configuration
    │   ├── Domain
    │   ├── Enums
    │   └── Exceptions
    ├── Exceptions
    ├── Filters
    ├── Helpers
    ├── Infrastructures
    │   ├── Repositories
    │   └── Specifications
    ├── Middleware
    ├── Modules
    │   ├── TodoItem
    │   │   ├── MappingProfiles
    │   │   ├── Models
    │   │   ├── Services
    │   │   └── Validators
    ├── Properties
    │   ├── launchSettings.json
    └── Shared
```

---

## 📝 **Additional notes**

## 📖 **Information**

### Package

```csharp
<ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.12"/>
    <PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="8.0.11"/>
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.12">
        <PrivateAssets>all</PrivateAssets>
        <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Newtonsoft.Json" Version="13.0.3"/>
    <PackageReference Include="Pomelo.EntityFrameworkCore.MySql.Json.Newtonsoft" Version="8.0.2" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.6.2"/>

    <PackageReference Include="AutoMapper" Version="13.0.1"/>
    <PackageReference Include="FluentValidation.AspNetCore" Version="11.3.0"/>
    <PackageReference Include="MailKit" Version="4.9.0"/>
    <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.12"/>
    <PackageReference Include="MimeKit" Version="4.9.0"/>

    <PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="8.0.12" />
    <PackageReference Include="Microsoft.AspNetCore.Identity.UI" Version="8.0.12" />
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.12" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.12">
        <PrivateAssets>all</PrivateAssets>
        <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.12">
        <PrivateAssets>all</PrivateAssets>
        <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Pomelo.EntityFrameworkCore.MySql" Version="8.0.2" />
    <PackageReference Include="Pomelo.EntityFrameworkCore.MySql.Design" Version="1.1.2" />

    <PackageReference Include="Microsoft.AspNetCore.Http.Abstractions" Version="2.3.0" />

    <Target Name="Husky" BeforeTargets="Restore;CollectPackageReferences" Condition="'$(HUSKY)' != 0">
        <Exec Command="dotnet tool restore" StandardOutputImportance="Low" StandardErrorImportance="High"/>
        <Exec Command="dotnet husky install" StandardOutputImportance="Low" StandardErrorImportance="High" WorkingDirectory=".."/>
    </Target>
</ItemGroup>

```

## ❔ **How to push**

- Role commit
  `{type}: #{issue_id} {subject}`
  - type: build | chore | ci | docs | feat | fix | perf | refactor | revert | style | test
  - subject: 'Write a short, imperative tense description of the change'
- Automatic: check lint and format pre-commit

- Example:

```bash
git commit -m "{type}: #{issue_id} {subject}"
```

Description
|**Types**| **Description** |
|:---| :--- |
|feat| A new feature|
|fix| A bug fix|
|docs| Documentation only changes|
|style| Changes that do not affect the meaning of the code (white-space, formatting, missing semi-colons, etc) |
|refactor| A code change that neither fixes a bug nor adds a feature |
|perf| A code change that improves performance |
|test| Adding missing tests or correcting existing tests |
|build| Changes that affect the build system or external dependencies (example scopes: gulp, broccoli, npm) |
|ci| 'Changes to our CI configuration files and scripts (example scopes: Travis, Circle, BrowserStack, SauceLabs) |
|chore| Other changes that don't modify src or test files |
|revert| Reverts a previous commit |

## 🔗 Workflow

### Feature Development 🚀

- **Branch Naming:** Create a branch from `dev` using the format `feature/[feature_name]`.
- **For example:** `feature/navbar`

### Bug Fixes During Development 🐞 🧑‍💻

- **Branch Naming:** Create a branch from `dev` using the format `fixbug/[bug_name]`.
- **For example:** `fixbug/typo`

### Bug Fixes in Production 🐞 🌏

- **Branch Naming:** Create a branch from `main` using the format `hotfix/[bug_name]`.
- **For example:** `hotfix/blur-image`

### Release a Version 🎢

- **Branch Naming:** Create a branch from `dev` using the format `release/[version]`.
- **Merge Process:** Merge the release branch into `main`.
- **For example:** `release/1.0.0`
