![KleeneStar](https://raw.githubusercontent.com/kleenestar-project/.github/main/docs/assets/img/banner.png)

# KleeneStar Workspace Template Concept

An empty workspace is one click away and worth almost nothing. What takes an afternoon is the decision underneath it: that a service desk needs a ticket, an incident, a service request, a problem, a change, a knowledge base and an announcement channel — and that three of those are the ones customers file. A workspace is not a name and a key. It is a set of classes, and choosing them one at a time afterwards is exactly the work somebody has already done elsewhere.

**KleeneStar** therefore lets a workspace be created from a **template**: a description of what a workspace is for and which classes it starts with. Templates are not rows in a table. They are **classes in a plugin**, discovered at runtime, and that is the whole point of the design.

Workspace templates bundle:
- A description of one shape of workspace — what it is for, what it is called, the key it proposes, the categories it belongs under.
- The classes a workspace created from it starts with, each with its object kind, its portal visibility and its access.
- Discovery from the installed plugins, so the catalogue is exactly as long as the installation says.
- A creation wizard that asks for the shape first and the paperwork second.

## Why Code, not Data

A template could have been an entity: a row an administrator edits, with child rows for its classes. It is not, and the reason is what a template *is*.

A template is knowledge about a domain, written down once and true for every installation that shares that domain. Kept as data it would have to be seeded into every new database, migrated whenever its shape changed, and held in step with each deployment by hand — and the first administrator to edit a seeded row would leave the installation permanently unable to take the improved version. Kept as code it is versioned, installable and removable by the same mechanism as everything else a plugin brings, and an installation that wants its own shapes ships its own plugin instead of editing somebody else's rows.

It follows that templates are **read-only at runtime**. What an administrator changes is the workspace a template produced, never the template. The two part company at the moment of creation and never meet again: nothing links a workspace back to the template it came from, because a workspace that drifted from its template is not wrong, it is finished.

|                    | Template as data                                  | Template as code
|--------------------|---------------------------------------------------|--------------------------------------------
| Lives in           | the installation's database                       | a plugin assembly
| Arrives by         | seeding                                           | installing the plugin
| Changes by         | editing a row                                     | shipping a version
| Removed by         | deleting rows, everywhere, by hand                | uninstalling the plugin
| Editable in the UI | yes — and then unable to take an update           | no
| Own shapes by      | editing the shipped rows                          | shipping a plugin beside them

## The Manager

`WorkspaceTemplateManager` is modelled on the framework's `FragmentManager`, and works the same way:

- it subscribes to the **plugin manager**, which is the source of truth about what is installed, rather than scanning the process;
- it scans the assembly of every plugin for public, non-abstract types implementing `IWorkspaceTemplate`, and instantiates them by reflection;
- it keeps its registrations **keyed by plugin**, so a plugin that goes away takes exactly its own templates with it;
- it announces what each plugin contributed to the log, and raises an event per registration and removal.

Two things differ from the fragment manager, and both follow from what a template is.

**Discovery is by interface, not by attribute.** A fragment declares a section, a scope, conditions and policies — it needs attributes because it has things to say about *where* it belongs. A template has nothing to declare beyond being one.

**A template is not bound to an application.** A fragment is a piece of one particular page and means nothing outside the application that page belongs to. A workspace is the installation's, not an application's, so the registry has one dimension where the fragment manager has two.

There is one ordering subtlety worth knowing about. The manager is constructed while the **core's** components are registered, which is before every other plugin has arrived. Neither discovery pass alone is therefore enough: the constructor sweeps the plugins already known, and the `AddPlugin` event catches the rest. A manager written with only one of the two would work in exactly half the installations.

## Applying a Template

Creating the classes is the manager's job (`Apply`) rather than the caller's, because it is the same act wherever it is triggered from — the wizard today, an import or a scripted setup tomorrow.

The classes are created **after** the workspace, because they belong to it: there is nothing to attach them to until it exists. A workspace stored while its classes failed is one an administrator can finish by hand; the other order would leave classes belonging to nothing.

Applying a template twice adds what is missing rather than a second set of everything. A name the workspace already carries is skipped, so a retried create — or a template applied to a workspace somebody had already set up by hand — does the useful thing instead of the destructive one.

An unknown template key creates nothing and raises nothing. That is the ordinary answer for a workspace whose template has since been uninstalled, and for every caller of the REST API that is not the wizard.

## The Creation Wizard

The "new workspace" dialog is a two-step wizard, in the same shape as the object wizard:

1. **Template** — the templates as cards, each stating what the workspace is for and which classes it starts with, searchable by name and description. The card projects the template's suggested key into the next step, which is the one field nobody has an opinion about until they have had to invent one.
2. **Details** — the name, the key, the categories, the description, who may see it.

The steps are in that order because the first is the decision and the second is paperwork.

The step always carries an **empty workspace** card besides whatever the plugins offer, and that card is visible whatever the search says. A workspace set up by hand has to stay one click away, and on an installation with no template plugin it is the only way through.

## Where the Pieces Live

| Concern                         | Where
|---------------------------------|--------------------------------------------------
| What a template is              | `IWorkspaceTemplate`, `WorkspaceTemplateClass` (`KleeneStar.Core/WebWorkspaceTemplate`)
| One registration                | `IWorkspaceTemplateContext`
| Discovery and application       | `IWorkspaceTemplateManager` / `WorkspaceTemplateManager`
| The wizard                      | `WorkspaceAddFormFragment`
| Applying on create              | `/api/1/workspaces` — `ApplyTemplate` in its `Create`
| The templates shipped           | the `KleeneStar.Templates` plugin

## Writing a Template

Implement the interface in a public, non-abstract class with a parameterless constructor. Nothing is registered; the manager finds it in the plugin's assembly:

```csharp
public sealed class ResearchTemplate : IWorkspaceTemplate
{
    public string Key => "acme.templates.research";
    public string Name => "acme.templates:template.research.name";
    public string Description => "acme.templates:template.research.description";
    public IIcon Icon => ImageIcon.FromString("/kleenestar/assets/icons/task.svg");
    public string SuggestedKey => "RES";
    public IEnumerable<string> Categories => ["Engineering"];
    public int Order => 100;

    public IEnumerable<WorkspaceTemplateClass> Classes =>
    [
        new() { Name = "Study",   Description = "…", Icon = "…", Kind = ObjectKind.Issue },
        new() { Name = "Dataset", Description = "…", Icon = "…", Kind = ObjectKind.Asset }
    ];
}
```

The key is stable and outlives renames of the class; the name and description are i18n keys; class names are **not** translated, because a class name is data an administrator renames rather than a caption of the product.

A plugin has to declare an application — the framework refuses one that belongs to nothing — so a template-only plugin names the application it extends rather than defining one of its own.

## Related Concepts

- [Workspaces](kleenestar.workspace.md) — what a template creates.
- [Classes](kleenestar.class.md) — what a template fills it with.
