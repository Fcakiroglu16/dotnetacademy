# Claude Code Permissions for .NET

Source code for the dotnetacademy.dev lesson [Claude Code Permissions for .NET Projects: Allow, Ask and Deny](https://dotnetacademy.dev/lesson/claude-code-permissions-for-net-projects-allow-ask-and-deny/361).

A small .NET 10 solution with a project-level Claude Code permission policy:

| Rule | Commands | Effect |
| --- | --- | --- |
| `allow` | `dotnet --info`, `dotnet build`, `dotnet test`, `dotnet test --filter *` | Run without asking |
| `ask` | `dotnet add *`, `dotnet publish *`, `dotnet ef database update *` | Always ask, even if another file allows them |
| `deny` | `Read(./.env)`, `Read(./**/appsettings.Production.json)`, `dotnet nuget push *` | Never run, no setting can allow them |

## Files

| Path | Purpose |
| --- | --- |
| `.claude/settings.json` | Shared, committed permission rules |
| `.claude/settings.local.json` | Your personal rules, git-ignored (not in the repo) |
| `CLAUDE.md` | Instructions for the model. They guide it but enforce nothing |
| `.env.example` | Copy to `.env` to test the `Read(./.env)` deny rule. The value is fake |
| `src/Shipping.Api`, `tests/Shipping.Tests` | The application and its tests |

## Try it

```bash
cp .env.example .env
dotnet build && dotnet test
claude            # accept the workspace trust dialog once, then run /permissions
```

Project `allow` rules are ignored until the folder is trusted. Deny and ask rules always apply.

Headless check of a single command, without your user settings:

```bash
claude -p 'Run `dotnet publish src/Shipping.Api -c Release -o out`' \
  --setting-sources project,local --output-format json
```

The JSON result lists the refused call under `permission_denials`.
