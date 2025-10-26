# KleeneStar Workspace Management Concept

This document specifies the management of workspaces as a central organizational and encapsulation instance within the **KleeneStar** system. Workspace management creates the multi-tenant framework required for the structured collection and logical separation of all modeled content.

A workspace acts as a self-contained container that bundles all associated data, such as type definitions, concrete instances of these types, their attributes, and relationships (links). It also isolates this data from other workspaces. This segmentation forms the foundation of the system's multi-tenancy and enables different organizational units, projects, or security domains to be cleanly separated from one another.

To promote standardization and consistency across multiple workspaces, the concept of blueprints is introduced. Any workspace can serve as a blueprint for other workspaces. A workspace derived from a blueprint inherits its structural configuration, particularly the definitions of Classess and Fields.

This inheritance is dynamic: changes to the type definitions in the blueprint are automatically propagated to all derived workspaces. This mechanism ensures that an entire group of workspaces is based on a uniform, centrally managed data model. At the same time, each derived workspace maintains its independence, as metadata such as name, description, or color-coding, as well as assigned permissions, can be configured individually. They are not inherited from the blueprint.

The functional scope of workspace management covers the entire lifecycle of a workspace. This includes the following core operations:

- **Create**: Creating new, isolated workspaces.
- **View and Navigate**: Clear presentation and quick switching between available workspaces.
- **Edit**: Adjusting the metadata and configurations of an existing workspace.
- **Clone**: Creating a copy of a workspace as a template.
- **Archive and Restore**: Temporarily decommissioning and reactivating workspaces to preserve data without cluttering the active work environment.
- **Delete**: Secure and traceable removal of no-longer-needed workspaces, taking into account retention periods.

The following state diagram visualizes these transitions:

```
╔══════════════════════════════════════════════════════════════════════════════════════╗
║                       KleeneStar Workspace State Diagram                             ║
╠══════════════════════════════════════════════════════════════════════════════════════╣
║                                                                                      ║
║                               ┌───────────────────┐                                  ║
║                               │     archive       ▼                                  ║
║                      new  ╔════════╗         ┌──────────┐                            ║
║                        ──►║ active ║         │ archived │                            ║
║                           ╚════════╝         └─┬──────┬─┘                            ║
║                             │    ▲   restore   │      │                              ║
║                             │    └─────────────┘      │                              ║
║                             │                         │                              ║
║                             │      ╔═════════╗        │                              ║
║                             └─────►║ deleted ║◄───────┘                              ║
║                                    ╚═════════╝                                       ║
║                                                                                      ║
╚══════════════════════════════════════════════════════════════════════════════════════╝
```

## Data Model

The **KleeneStar** Core Data Model forms the structural foundation for managing identities, roles, and permissions. It is based on a clearly modular architecture that distinguishes between type definitions, concrete instances, and semantic extensions. The components of the permission model (e.g., Position, Role, Resource, Policy) can be defined as Classes. Their specific manifestations (e.g., "Head of Marketing", "Admin") are Objects with corresponding Values (e.g., description, assigned group). Relationships such as "Position → Role" or "Role → Resource" are modeled via Links. Comments, versioning, and file references enable additional documentation and traceability.

```
╔══════════════════════════════════════════════════════════════════════════════════════╗
║                             KleeneStar Core Data Model                               ║
╠══════════════════════════════════════════════════════════════════════════════════════╣
║                                                                                      ║
║       ┌─────────────┐ *           * ┌───────┐ 1          * ┌───────┐                 ║
║       │ Workspace   ├──────────────►│ Class │◄─────────────┤ Field │                 ║
║       └─────┬───────┘               └───────┘              └───────┘                 ║
║             │ 1                         ▲ 1                    ▲ 1                   ║
║             └──────────────────────┐    │                      │                     ║
║                                    ▼ *  │ *                    │ *                   ║
║              ┌──────┐ *        2 ┌──────┴─┐ 1            * ┌───┴───┐                 ║
║              │ Link ├───────────►│ Object │◄───────────────┤ Value │                 ║
║              └──────┘            └────────┘                └───────┘                 ║
║                                   ▲ 1   ▲ 1                    ▲ 1                   ║
║                     ┌─────────────┘     │                      │                     ║
║                     │ *                 │ *                    │ *                   ║
║                ┌────┴────┐         ┌────┴────┐         ┌───────┴───────┐             ║
║                │ Comment │         │ Version │         │ FileReference │             ║
║                └─────────┘         └─────────┘         └───────────────┘             ║
║                                                                                      ║
╚══════════════════════════════════════════════════════════════════════════════════════╝
```

## Software Architecture

The application's architecture follows a modular, decoupled design. At its core is the `WorkspaceManager`, which is exclusively responsible for the lifecycle and access to all workspaces. Through the `IWorkspaceManager` interface, it manages a collection of `IWorkspace` instances.

The data structure of a workspace is defined by the `IWorkspace` interface and its concrete implementation in the `Workspace` class. These objects contain central attributes such as `Key`, `Name`, `Description`, and lifecycle information. New workspace instances are created exclusively by the `WorkspaceManager`. Direct access to internal data structures is not permitted by the system. Instead, the interface acts as a controlled intermediary for all interactions—a model that reliably protects data integrity.

For reactive, loosely coupled communication, the `WorkspaceManager` provides the `AddWorkspace` and `RemoveWorkspace` events. Other components can subscribe to these events and react to changes without creating a direct dependency on the manager. This event-driven mechanism promotes high cohesion while maintaining modularity. Additionally, the events are made available system-wide via the `WebExpress-EventManager`.

The `WorkspaceManager` also handles several server-side tasks that are essential for scalability, security, and traceability. This includes the persistent storage of all workspaces in a transaction-safe, versioned repository. At system startup, all stored workspaces are loaded, and all indexes and event subscriptions are initialized.

To support high-performance full-text and metadata searches, a server-side reverse index is created for each workspace. This index includes keywords from the `Name`, `Description`, user-defined tags, and structured metadata such as creation date and status. The index is continuously updated and enables fast, context-aware searches across the entire workspace inventory.

Another central aspect is access control. The `WorkspaceManager` checks the permissions of the calling module or user with every request. Access restrictions can be defined via policies. The context itself can contain temporary rights, audit trails, or context-dependent filters. This allows for the implementation of time-limited write permissions or differentiated read permissions.

To ensure transparency and traceability, every relevant action related to workspaces is logged by an integrated audit system. It documents accesses, changes, context switches, and permission checks in a structured format. The logs contain timestamps, user identities, affected workspace keys, and the type of action (e.g., creation, modification). This data is used for analysis, error diagnosis, compliance auditing, and state restoration.

```
╔KleeneStar.Core═══════════════════════════════════════════════════════════════════════╗
║                                                                                      ║
║                              ┌────────────────────┐                                  ║
║                              │ <<Interface>>      │                                  ║
║                              │ IComponentManager  │                                  ║
║                              ├────────────────────┤                                  ║
║                              └────────────────────┘                                  ║
║                                       Δ                                              ║
║                                       ¦                                              ║
║                                       ¦                                              ║
║                     ┌─────────────────┴─────────────────────┐                        ║
║                     │ <<Interface>>                         │                        ║
║         ┌-----------┤ IWorkspaceManager                     │                        ║
║         ¦           ├───────────────────────────────────────┤                        ║
║         ¦           │ AddWorkspace:Event                    │                        ║
║         ¦           │ RemoveWorkspace:Event                 │                        ║
║         ¦           ├───────────────────────────────────────┤ 1                      ║
║         ¦           │ Workspaces:IEnumerable<IWorkspace>    ├───────┐                ║
║         ¦           ├───────────────────────────────────────┤       │                ║
║         ¦           │ AddWorkspace(Workspace):IWorkspace    │       │                ║
║         ¦           │ GetWorkspaces(filter):                │       │                ║
║         ¦           │   IEnumerable<IWorkspace>             │       │                ║
║         ¦           │ CloneWorkspace(IWorkspace):IWorkspace │       │                ║
║         ¦           │ DeleteWorkspace(IWorkspace):bool      │       │                ║
║         ¦           └───────────────────────────────────────┘       │                ║
║         ¦                                                           │                ║
║         ¦             ┌───────────────┐   ┌────────────────┐        │                ║
║         ¦             │ <<Interface>> │   │ <<Interface>>  │        │                ║
║         ¦             │ IModel        │   │ IIndexItem     │        │                ║
║         ¦             ├───────────────┤   ├────────────────┤        │                ║
║         ¦             └───────────────┘   │ Id: Guid       │        │                ║
║         ¦                    Δ            └────────────────┘        │                ║
║         ¦                    ¦                   Δ                  │                ║
║         ¦                    └--------┬----------┘                  │                ║
║         ¦                             ¦                             │                ║
║         ¦           ┌─────────────────┴──────────────────┐ *        │                ║
║         ¦           │ <<Interface>>                      │◄─────────┘                ║
║         ¦           │ IWorkspace                         │                           ║
║         ¦           ├────────────────────────────────────┤                           ║
║         ¦           │ Key:String                         │                           ║
║         ¦           │ Name:String                        │                           ║
║         ¦           │ Icon:IIcon                         │                           ║
║         ¦           │ Description:String                 │                           ║
║         ¦           │ Created:DateTime                   │                           ║
║         ¦           │ Updated:DateTime                   │                           ║
║         ¦           │ Archived:Bool                      │                           ║
║         ¦           │ Classes:                           │                           ║
║         ¦           │   IEnumerable<IClass>              │                           ║
║         ¦           │ Objects:                           │                           ║
║         ¦           │   IEnumerable<IObject>             │                           ║
║         ¦           │ AccessibleWorkspaces:              │                           ║
║         ¦           │   IEnumerable<IWorkspace>          │                           ║
║         ¦           │ PermissionsProfiles:               │                           ║
║         ¦           │   IEnumerable<IPermissionsProfile> │                           ║
║         ¦           └────────────────────────────────────┘                           ║
║         ¦                             Δ                                              ║
║         ¦                             ¦                                              ║
║         ¦                             ¦                                              ║
║         ¦ create    ┌─────────────────┴──────────────────┐                           ║
║         └----------►│ Workspace                          │                           ║
║                     ├────────────────────────────────────┤                           ║
║                     │ Key:String                         │                           ║
║                     │ Name:String                        │                           ║
║                     │ Icon:IIcon                         │                           ║
║                     │ Description:String                 │                           ║
║                     │ Created:DateTime                   │                           ║
║                     │ Updated:DateTime                   │                           ║
║                     │ Archived:Bool                      │                           ║
║                     │ Classes:                           │                           ║
║                     │   IEnumerable<IClass>              │                           ║
║                     │ Objects:                           │                           ║
║                     │   IEnumerable<IObject>             │                           ║
║                     │ AccessibleWorkspaces:              │                           ║
║                     │   IEnumerable<IWorkspace>          │                           ║
║                     │ PermissionsProfiles:               │                           ║
║                     │   IEnumerable<IPermissionsProfile> │                           ║
║                     └────────────────────────────────────┘                           ║
║                                                                                      ║
╚══════════════════════════════════════════════════════════════════════════════════════╝
```

## UI Concepts and Pages

The following UI mockups translate the abstract data models and lifecycle rules of workspace management into a concrete and tangible user experience. The goal is to ensure intuitive, efficient, and secure interaction with workspaces. All UI concepts presented here are based on the established and consistent UI patterns of the **KleeneStar** WebApp to ensure high recognizability and a short learning curve.

These mockups serve as a blueprint for the final design and specify navigation, the arrangement of controls, and the display of various system states. They illustrate how users are guided through the application to perform operations such as creating, managing, or archiving workspaces.

### Global Workspace Dropdown (Header)

The global workspace dropdown is a central and permanently available component in the application header. It allows for quick and context-aware switching between different workspaces without leaving the current view. The control displays the name of the active workspace and, upon interaction, opens a dropdown menu that provides a searchable list of all available workspaces. To increase efficiency, recently used workspaces are prominently placed.

```
╔WebAppPage════════════════════════════════════════════════════════════════════════════╗
║┌Header──────────────────────────────────────────────────────────────────────────────┐║
║│ * KleeneStar     Workspace ▼                                                       │║
║└────────────────────¦───────────────────────────────────────────────────────────────┘║
║┌Breadcrumb────────┌─┴────────────────┐──────────────────────────────────────────────┐║
║│ / Site / ...     │┌────────────────┐│                                              │║
║└──────────────────││ Search         ││──────────────────────────────────────────────┘║
║┌Workspace ────────│└────────────────┘│──────────────────────────────────────────────┐║
║│                  │ Workspace 0      │                                              │║
║│  [Icon]          │ Workspace 1      │                     [ Search ] [+ AddObject] │║
║│  [Name]          │ ...              │                                              │║
║│                  │ Workspace n      │     | Tel.  | Description                    │║
║│                  ├──────────────────┤-----|-------|------------------------------- │║
║│ Class            │ Manage Workspace │     | 555-1 | Head of Sales              […] │║
║│ ├─ xxxxxx        │ + Add Workspace  │lder | 555-7 | IT Administrator           […] │║
║│ ├─ yyyyyyyyy     │ <section>        │     | 555-3 | HR Manager                 […] │║
║│ └─ zzzzz         └──────────────────┘e    | 555-4 | System Architect           […] │║
║│                      │░│ Anna Mock        | 555-8 | DevOps Lead                […] │║
║│                      │<│                                                           │║
║│                      │<│                                   ‹ Prev  1  2  3  Next › │║
║│                      │<│                                                           │║
║│                      │░│                                                           │║
║│                      │░│                                                           │║
║│                      │░│                                                           │║
║│                      │░│                                                           │║
║│                      │░│                                                           │║
║│                      │░│                                                           │║
║├──────────────────────┤░│                                                           │║
║│ [+] | [Setting]   << │░│                                                           │║
║└──────────────────────┘ └───────────────────────────────────────────────────────────┘║
║┌Footer──────────────────────────────────────────────────────────────────────────────┐║
║│                                                                                    │║
║└────────────────────────────────────────────────────────────────────────────────────┘║
╚══════════════════════════════════════════════════════════════════════════════════════╝
```

### Workspace Management (Page)

A dedicated management page exists for the comprehensive control of all workspaces in the system. This page serves as a central hub, offering a complete overview and powerful tools for managing the entire workspace inventory. In contrast to context-specific components like the global dropdown, this view provides a global perspective and offers advanced search and filtering capabilities.

The main component of this page is a tabular list of all workspaces. Each row represents a workspace and displays its most important attributes, such as name, key, current status, and type (e.g., standard or blueprint). To facilitate navigation even in extensive inventories, powerful search and filter functions are integrated directly into the interface. Furthermore, the page supports bulk operations, allowing administrators to select multiple workspaces to efficiently perform actions like archiving or deleting for the entire selection at once. A clearly visible button for creating a new workspace completes the feature set and serves as the primary entry point for expanding the system.

```
╔WebAppPage════════════════════════════════════════════════════════════════════════════╗
║┌Header──────────────────────────────────────────────────────────────────────────────┐║
║│ * KleeneStar     Workspace ▼                                                       │║
║└────────────────────────────────────────────────────────────────────────────────────┘║
║┌Breadcrumb──────────────────────────────────────────────────────────────────────────┐║
║│ / Workspaces                                                                       │║
║└────────────────────────────────────────────────────────────────────────────────────┘║
║┌Workspace Category────┐ ┌Workspaces─────────────────────────────────────────────────┐║
║│                      │░│                                                           │║
║│ - All                │░│                              [ Search ] [+ Add Workspace] │║
║│ - Category 0         │░│                                                           │║
║│ - Category 1         │░│ Name             | Key         | Status   | ...           │║
║│ - ...                │░│------------------|-------------|----------|-------------- │║
║│ - Category n         │░│ Sales Operations | sales-ops   | active   | ...       […] │║
║│                      │░│ Engineering      | engineering | archived | ...       […] │║
║│                      │░│ Marketing        | marketing   | active   | ...       […] │║
║│                      │░│                                                        ¦  │║
║│                      │░│                                   ‹ Prev  ┌────────────┴┐ │║
║│                      │<│                                           │ Edit        │ │║
║│                      │<│                                           │ Clone       │ │║
║│                      │<│                                           │ Permissions │ │║
║│                      │░│                                           │ <section>   │ │║
║│                      │░│                                           ├─────────────┤ │║
║│                      │░│                                           │ Delete      │ │║
║│                      │░│                                           └─────────────┘ │║
║│                      │░│                                                           │║
║│                      │░│                                                           │║
║├──────────────────────┤░│                                                           │║
║│                   << │░│                                                           │║
║└──────────────────────┘ └───────────────────────────────────────────────────────────┘║
║┌Footer──────────────────────────────────────────────────────────────────────────────┐║
║│                                                                                    │║
║└────────────────────────────────────────────────────────────────────────────────────┘║
╚══════════════════════════════════════════════════════════════════════════════════════╝
```

### Workspace Management (Sidebar)

Complementing the global dropdown, the sidebar offers a detailed view and direct interaction options for the currently selected workspace. This context-sensitive component is prominently placed in the main navigation or a dedicated management view and serves as the central dashboard for the respective workspace.

The sidebar visualizes essential metadata at a glance, such as the name, an icon, and the current status (e.g., active, archived, or whether it is a blueprint). Additionally, it provides direct access to central management functions. Actions like editing settings, cloning, archiving, or deleting the workspace are immediately accessible via clearly labeled buttons.

```
╔WebAppPage════════════════════════════════════════════════════════════════════════════╗
║┌Header──────────────────────────────────────────────────────────────────────────────┐║
║│ * KleeneStar     Workspace ▼                                                       │║
║└────────────────────────────────────────────────────────────────────────────────────┘║
║┌Breadcrumb──────────────────────────────────────────────────────────────────────────┐║
║│ / Workspace 0                                                                      │║
║└────────────────────────────────────────────────────────────────────────────────────┘║
║┌Workspace─────────────┐ ┌Workspace Content──────────────────────────────────────────┐║
║│[Name]                │░│                                                           │║
║│                      │░│ Class xxxxxx                     [ Search ] [+ AddObject] │║
║│      [Icon]          │░│                                                           │║
║│                      │░│ Name             | Tel.  | Description                    │║
║│                      │░│------------------|-------|------------------------------- │║
║│ Class                │░│ John Sample      | 555-1 | Head of Sales              […] │║
║│ ├─ xxxxxx        […] │░│ Jane Placeholder | 555-7 | IT Administrator           […] │║
║│ ├─ yyyyyyyyy     […] │░│ Mark Demo        | 555-3 | HR Manager                 […] │║
║│ └─ zzzzz         […] │░│ Emily Example    | 555-4 | System Architect           […] │║
║│                      │░│ Anna Mock        | 555-8 | DevOps Lead                […] │║
║│                      │<│                                                           │║
║│       ┌─────────────┐│<│                                   ‹ Prev  1  2  3  Next › │║
║│       │ Edit        ││<│                                                           │║
║│       │ Clone       ││░│                                                           │║
║│       │ Permissions ││░│                                                           │║
║│       │ <section>   ││░│                                                           │║
║│       ├─────────────┤│░│                                                           │║
║│       │ Delete      ││░│                                                           │║
║│       └┬────────────┘│░│                                                           │║
║├────────¦─────────────┤░│                                                           │║
║│ [+] | [Setting]   << │░│                                                           │║
║└──────────────────────┘ └───────────────────────────────────────────────────────────┘║
║┌Footer──────────────────────────────────────────────────────────────────────────────┐║
║│                                                                                    │║
║└────────────────────────────────────────────────────────────────────────────────────┘║
╚══════════════════════════════════════════════════════════════════════════════════════╝
```

### Workspace Management - New/Edit (Modal)

A modal dialog window is used for creating a new or editing an existing workspace. This approach ensures a focused and distraction-free interaction by concentrating the editing process on a single, well-defined task without leaving the context of the parent management page. The modal is opened through dedicated actions such as the "+ New Workspace" button on the management page, an "Edit" command within the workspace table, or via the "Settings" button in the workspace sidebar.

The content of the modal dynamically adapts to the respective use case. In edit mode, the form fields are pre-filled with the data of the selected workspace. Critical, immutable fields such as the unique key ("Key") are read-only in this mode to maintain system integrity and referential consistency. When creating a new workspace, users have the option to derive it from an existing blueprint, which helps standardize configurations and promote system consistency.

```
╔WebAppPage════════════════════════════════════════════════════════════════════════════╗
║┌Header──────────────────────────────────────────────────────────────────────────────┐║
║│ * KleeneStar     Workspace ▼                                                       │║
║└─────╔WorkspaceModal══════════════════════════════════════════════════════════╗─────┘║
║┌Bread║┌Form──────────────────────────────────────────────────────────────────┐║─────┐║
║│ / Wo║│ Add Workspace / Edit Workspace                                       │║     │║
║└─────║├──────────────────────────────────────────────────────────────────────┤║─────┘║
║┌Works║│                                                                      │║─────┐║
║│[Name║│                  Name: [Workspace A                                ] │║     │║
║│     ║│                   Key: [ws0                                        ] │║ect] │║
║│     ║│              Category: [                                           ] │║     │║
║│     ║│             Blueprint: [None                                      ▼] │║     │║
║│     ║│                Active: [✓]                                           │║---- │║
║│ Clas║│ Accessible Workspaces: [Workspace B, Workspace C                  ▼] │║ […] │║
║│ ├─ x║│           Description: [                                           ] │║ […] │║
║│ ├─ y║│                                                                      │║ […] │║
║│ └─ z║│                                                                      │║ […] │║
║│     ║│                                                                      │║ […] │║
║│     ║│                                                                      │║     │║
║│     ║│                                                                      │║xt › │║
║│     ║│                                                                      │║     │║
║│     ║│                                                                      │║     │║
║│     ║│                                                                      │║     │║
║│     ║│                                                                      │║     │║
║│     ║│                                                                      │║     │║
║│     ║│                                                                      │║     │║
║│     ║└──────────────────────────────────────────────────────────────────────┘║     │║
║├─────║                                                                        ║     │║
║│ [+] ║                                                       [Save] [Cancel]  ║     │║
║└─────║                                                                        ║─────┘║
║┌Foote╚════════════════════════════════════════════════════════════════════════╝─────┐║
║│                                                                                    │║
║└────────────────────────────────────────────────────────────────────────────────────┘║
╚══════════════════════════════════════════════════════════════════════════════════════╝
```

### Workspace Management – Clone (Modal)

Cloning a workspace allows for the rapid replication of existing structures and is particularly useful for reusing proven configurations. The function is provided via a separate modal dialog window, which can be called from the detail view or the management page.

When cloning, a new workspace is created whose attributes (such as name, description, tags, and metadata) are copied from the original. System-critical properties like the key, creation time, and permissions are newly generated or explicitly requested. The modal allows the user to customize the name and description of the new workspace. Optionally, tags and context-dependent settings can be adopted or modified.

After confirmation, the new workspace is created and automatically integrated into the existing workspace list.

```
╔WebAppPage════════════════════════════════════════════════════════════════════════════╗
║┌Header──────────────────────────────────────────────────────────────────────────────┐║
║│ * KleeneStar     Workspace ▼                                                       │║
║└─────╔CloneModal══════════════════════════════════════════════════════════════╗─────┘║
║┌Bread║┌Form──────────────────────────────────────────────────────────────────┐║─────┐║
║│ / Wo║│ Clone Workspace                                                      │║     │║
║└─────║├──────────────────────────────────────────────────────────────────────┤║─────┘║
║┌Works║│                                                                      │║─────┐║
║│[Name║│ You are about to clone the workspace 'Sales Operations'.             │║     │║
║│     ║│ Please adjust the details for the new workspace below.               │║ect] │║
║│     ║│                                                                      │║     │║
║│     ║│ Workspace Name: [ Sales Operations (Copy)                        ]   │║     │║
║│     ║│            Key: [ newkey                                         ]   │║---- │║
║│ Clas║│    Description: [ Copy of sales-related workflows and assets.    ]   │║ […] │║
║│ ├─ x║│                                                                      │║ […] │║
║│ ├─ y║│   Include structure: [✓]                                             │║ […] │║
║│ └─ z║│ Include permissions: [✓]                                             │║ […] │║
║│     ║│                                                                      │║ […] │║
║│     ║│                                                                      │║     │║
║│     ║│                                                                      │║xt › │║
║│     ║│                                                                      │║     │║
║│     ║│                                                                      │║     │║
║│     ║└──────────────────────────────────────────────────────────────────────┘║     │║
║├─────║                                                                        ║     │║
║│ [+] ║                                                      [Clone] [Cancel]  ║     │║
║└─────║                                                                        ║─────┘║
║┌Foote╚════════════════════════════════════════════════════════════════════════╝─────┐║
║│                                                                                    │║
║└────────────────────────────────────────────────────────────────────────────────────┘║
╚══════════════════════════════════════════════════════════════════════════════════════╝
```

### Workspace Management - Delete (Modal)

Deleting a workspace is a critical and irreversible operation that requires explicit confirmation from the user. To prevent accidental deletions, a modal dialog window is used. This modal is activated when the user initiates the delete action from the workspace detail view or the management page.

The dialog window clearly indicates which workspace is intended for deletion by explicitly stating its name and key. As an additional security measure, the user must type the key of the workspace to be deleted into a designated input field. Only when the input matches the workspace's key does the final delete button become active. This mechanism ensures that the action is performed consciously and deliberately. In addition to the confirmation button, the modal offers a clear option to cancel the process, allowing the window to be closed without making any changes.

```
╔WebAppPage════════════════════════════════════════════════════════════════════════════╗
║┌Header──────────────────────────────────────────────────────────────────────────────┐║
║│ * KleeneStar     Workspace ▼                                                       │║
║└─────╔DeleteModal═════════════════════════════════════════════════════════════╗─────┘║
║┌Bread║┌Form──────────────────────────────────────────────────────────────────┐║─────┐║
║│ / Wo║│ Delete Workspace                                                     │║     │║
║└─────║├──────────────────────────────────────────────────────────────────────┤║─────┘║
║┌Works║│                                                                      │║─────┐║
║│     ║│ Are you sure you want to delete the workspace 'Sales Operations'?    │║     │║
║│  [Ic║│ This action cannot be undone.                                        │║Add] │║
║│  [Na║│                                                                      │║     │║
║│     ║│ To confirm, please type 'sales-ops' in the box below:                │║     │║
║│     ║│ [                                                                 ]  │║---- │║
║│ Clas║│                                                                      │║ […] │║
║│ ├─ x║│                                                                      │║ […] │║
║│ ├─ y║│                                                                      │║ […] │║
║│ └─ z║│                                                                      │║ […] │║
║│     ║│                                                                      │║ […] │║
║│     ║│                                                                      │║     │║
║│     ║│                                                                      │║xt › │║
║│     ║│                                                                      │║     │║
║│     ║│                                                                      │║     │║
║│     ║│                                                                      │║     │║
║│     ║│                                                                      │║     │║
║│     ║│                                                                      │║     │║
║│     ║│                                                                      │║     │║
║│     ║└──────────────────────────────────────────────────────────────────────┘║     │║
║├─────║                                                                        ║     │║
║│ [+] ║                                                     [Delete] [Cancel]  ║     │║
║└─────║                                                                        ║─────┘║
║┌Foote╚════════════════════════════════════════════════════════════════════════╝─────┐║
║│                                                                                    │║
║└────────────────────────────────────────────────────────────────────────────────────┘║
╚══════════════════════════════════════════════════════════════════════════════════════╝
```

### Workspace Management - Permissions Management (Modal)

To manage the access rights of a workspace, a modal dialog window is used, which serves as the central interface for assigning groups to context-specific policies. This modal allows for granular control over which groups may act with which roles and permissions within a workspace.

Access to the modal is granted via the "Permissions" button in the workspace management view. Displaying and using the dialog requires the `workspace_manage_profiles` permission. Within the modal, administrators can select groups and assign them suitable policies, for instance, for read access, editing, or administrative control.

Assignments are displayed in a tabular overview and can be adjusted or removed at any time.

```
╔WebAppPage════════════════════════════════════════════════════════════════════════════╗
║┌Header──────────────────────────────────────────────────────────────────────────────┐║
║│ * KleeneStar     Workspace ▼                                                       │║
║└─────╔PermissionsModal════════════════════════════════════════════════════════╗─────┘║
║┌Bread║┌Form──────────────────────────────────────────────────────────────────┐║─────┐║
║│ / Wo║│  Manage Permissions for 'Sales Operations'                           │║     │║
║└─────║├──────────────────────────────────────────────────────────────────────┤║─────┘║
║┌Works║│                                                                      │║─────┐║
║│     ║│  Assign Group: [Admin ▼]                                             │║     │║
║│  [Ic║│        Policy: [workspace_admin_policy ▼]                            │║Add] │║
║│  [Na║│                                                                      │║     │║
║│     ║│  [+ Assign]                                                          │║     │║
║│     ║│                                                          [ Search ]  │║---- │║
║│ Clas║│                                                                      │║ […] │║
║│ ├─ x║│ Assigned Group       | Effective Policy                              │║ […] │║
║│ ├─ y║│----------------------|-----------------------------------------------│║ […] │║
║│ └─ z║│ Admin                | workspace_admin_policy                      X │║ […] │║
║│     ║│ User                 | workspace_view_policy                       X │║ […] │║
║│     ║│                                                                      │║     │║
║│     ║│                                             ‹ Prev  1  2  3  Next ›  │║xt › │║
║│     ║│                                                                      │║     │║
║│     ║│                                                                      │║     │║
║│     ║│                                                                      │║     │║
║│     ║│                                                                      │║     │║
║│     ║│                                                                      │║     │║
║│     ║│                                                                      │║     │║
║│     ║└──────────────────────────────────────────────────────────────────────┘║     │║
║├─────║                                                                        ║     │║
║│ [+][║                                                                [Done]  ║     │║
║└─────║                                                                        ║─────┘║
║┌Foote╚════════════════════════════════════════════════════════════════════════╝─────┐║
║│                                                                                    │║
║└────────────────────────────────────────────────────────────────────────────────────┘║
╚══════════════════════════════════════════════════════════════════════════════════════╝
```

## Sitemap

The sitemap defines the hierarchical structure and navigation paths of the user interface for workspace management. It ensures a clear organization of the pages, serves as the basis for routing within the web application, and is structured as follows:

|Path                            |Page                  |Description
|--------------------------------|----------------------|-------------------------------------------------------------
|`/`                             |Dashboard             |Central entry point of the application.
|`/workspaces`                   |Workspace Management  |Overview of all workspaces with search, filter, and management functions.
|`/workspaces/add`               |Workspace Creation    |Form for creating a new workspace.
|`/workspaces/{key}`             |Workspace Detail View |Detailed view and actions for a single workspace.
|`/workspaces/{key}/edit`        |Workspace Editing     |Form for editing the metadata of an existing workspace.
|`/workspaces/{key}/clone`       |Workspace Cloning     |Dialog for replicating an existing workspace with customizable fields.
|`/workspaces/{key}/delete`      |Workspace Deletion    |Modal for confirming and executing the irreversible deletion of a workspace.
|`/workspaces/{key}/permissions` |Workspace Permissions |Modal for managing profiles (group-policy assignments) for a specific workspace.

## API Interfaces (REST Endpoints)

For programmatic interaction, integration of third-party systems, and automation purposes, workspace management provides a standardized REST API. This interface follows REST principles and uses JSON as the data exchange format. Authentication and authorization are handled by mechanisms provided by the central `IdentityManager`, typically via API tokens or OAuth. Standard HTTP status codes are used to signal the success or failure of a request.

The management of workspaces is handled via the following endpoints:

|Endpoint                                   |HTTP Method |Description
|-------------------------------------------|------------|------------------------------------------------------------
|`/api/workspaces`                          |GET         |Lists all available workspaces. The results are paginated and can be filtered by status and sorted.
|`/api/workspaces`                          |POST        |Creates a new workspace. Requires at least a `name` and a system-wide unique `key` in the request body.
|`/api/workspaces/{key}`                    |GET         |Retrieves the detailed information of a specific workspace by its key.
|`/api/workspaces/{key}`                    |PUT         |Updates the metadata (e.g., `name`, `description`) of an existing workspace. The key is immutable.
|`/api/workspaces/{key}`                    |DELETE      |Deletes a workspace.
|`/api/workspaces/{key}/archive`            |POST        |Archives a workspace, placing it in a read-only state.
|`/api/workspaces/{key}/restore`            |POST        |Restores an archived or temporarily deleted workspace, setting its status to `active`.
|`/api/workspaces/{key}/profiles`           |GET         |Lists all profiles (group-policy assignments) for the specified workspace. Requires the workspace:manage_profiles permission.
|`/api/workspaces/{key}/profiles`           |POST        |Creates a new profile, assigning a group to a policy within the workspace. The request body must contain groupId and policyId.
|`/api/workspaces/{key}/profiles/{groupId}` |DELETE      |Deletes a profile for a specific group from the workspace, thereby revoking the group's permissions.

Standard error responses include `400 Bad Request` for validation errors (e.g., a key that is already taken), `401 Unauthorized` for missing authentication, `403 Forbidden` for insufficient permissions, and `404 Not Found` if the requested resource does not exist. A successful creation (POST) is acknowledged with `201 Created`, while a successful deletion (DELETE) results in a `204 No Content` response.

## Workspace Events

Workspace management utilizes an event-driven architecture model to communicate state changes transparently and reactively throughout the system. Events are published via the `WebExpress-EventManager`, which acts as the central event backbone. This allows other modules, plugins, or external systems to subscribe to relevant changes without being directly coupled to the `WorkspaceManager`.

The following events are published by the `WorkspaceManager` via the `WebExpress-EventManager`:
|Event Name          |Description
|--------------------|-----------------------------------------------------------------------------
|`WorkspaceAdded`    |Triggered when a new workspace has been successfully created.
|`WorkspaceUpdated`  |Signals changes to the metadata of an existing workspace.
|`WorkspaceRemoved`  |Indicates the permanent deletion of a workspace.
|`WorkspaceArchived` |Marks a workspace as archived, placing it in a passive state.
|`WorkspaceRestored` |Reports the restoration of a previously archived or deleted workspace.
|`WorkspaceCloned`   |Triggered when a workspace has been successfully duplicated.

The events contain a structured payload, including:
- The unique workspace key
- Timestamp of the action
- User or module context
- Type and source of the action

Through integration with the `WebExpress-EventManager`, these events are available both within the application and to connected subsystems.

## Permissions Model

The global permissions model of **KleeneStar** is applied context-specifically to individual workspaces. The connection between the globally defined groups and policies is established through a Profile, which is valid exclusively within a specific workspace.

A profile defines which policy a global group receives within a specific workspace. It functions as a context-aware role assignment and enables granular and flexible rights management.

- **Principle:** A user receives the rights defined by a policy for a workspace if they are a member of a group for which a corresponding profile (`Group -> Policy`) exists in that workspace.
- **Flexibility:** The same global group (e.g., "Marketing") can be assigned the `workspace_view_policy` (read-only access) in Workspace A and the `workspace_edit_policy` (write access) in Workspace B.
- **Management:** Users with administrative rights for a workspace (e.g., through the `workspace_admin_policy`) can create, edit, and delete profiles. This means they can manage the assignment of policies to groups for their workspace.

The following table lists the granular permissions required for comprehensive control of workspace management.

| Permission                  | Description
| --------------------------- | -----------------------------------------------------------------------------------
| `workspace_create`          | Allows the creation of new, isolated workspaces.
| `workspace_read`            | Grants read access to the metadata of a workspace (name, description, status, etc.).
| `workspace_update`          | Authorizes the modification of an existing workspace's metadata.
| `workspace_delete`          | Allows the permanent deletion of a workspace.
| `workspace_archive`         | Permits the archiving of an active workspace.
| `workspace_restore`         | Enables the restoration of an archived workspace.
| `workspace_clone`           | Authorizes the duplication of an existing workspace.
| `workspace_manage_profiles` | Allows the management of profiles (assignment of policies to groups) for a workspace.
| `workspace_read_content`    | Grants read access to the contents of a workspace (entities, attributes, etc.).
| `workspace_write_content`   | Allows the creation, editing, and deletion of content within a workspace.

These permissions are bundled into logical policies to represent typical use cases and responsibilities. The policies can be assigned to global groups within a workspace profile.

|Policy                     |Description                                                 |Included Permissions
|---------------------------|------------------------------------------------------------|-------------------------
|`workspace_admin_policy`   |Complete administrative control over a workspace.           |`workspace_read`, `workspace_update`, `workspace_delete`, `workspace_archive`, `workspace_restore`, `workspace_clone`, `workspace_manage_profiles`, `workspace_read_content`, `workspace_write_content`
|`workspace_edit_policy`    |Authorizes the management of a workspace's contents.        |`workspace_read`, `workspace_read_content`, `workspace_write_content`
|`workspace_view_policy`    |Grants read-only access to a workspace and its contents.    |`workspace_read`, `workspace_read_content`
|`workspace_creator_policy` |A global policy that allows the creation of new workspaces. |`workspace_create`

## Conclusion

The document "KleeneStar Workspace Management" provides the conceptual foundation for a reference implementation. It outlines core functional requirements across data modeling, architecture, and user interfaces, and defines the lifecycle of a workspace including a flexible permissions model based on context-specific profiles. As a high-level blueprint, it intentionally omits technical detail in key areas. Topics such as persistent storage, concurrent access handling, and validation logic are left open and must be defined during implementation. The focus lies on core functional flows. Long-running operations like cloning or archiving large workspaces, asynchronous processing, and user notifications are not covered. Similarly, retention logic from the state diagram and the audit system design remain unspecified. The document thus offers a solid conceptual base while leaving critical implementation aspects open. The reference implementation is expected to close these gaps with concrete solutions and validate the practical feasibility of the proposed models.