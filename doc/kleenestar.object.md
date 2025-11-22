![KleeneStar](https://raw.githubusercontent.com/kleene-star/.github/main/docs/assets/img/banner.png)

# KleeneStar Object Management Concept

Object management in **KleeneStar** forms the central foundation for structured, traceable, and scalable data management. An object represents a concrete instance of a class and corresponds to an individual record, such as an incident, a document, or any business entity. Objects are always assigned to a workspace and inherit its structural, security-related, and organizational constraints.

An object aggregates business data values, relationships to other objects, and accompanying metadata such as comments, versioning, file attachments, or audit information. This combination enables a holistic view of the data context and supports both operational and analytical requirements.

The object management is fully implemented server-side, multi-tenant, and audit-proof. The lifecycle of an object in **KleeneStar** essentially consists of the states "active" and "archived". Within the active state, however, an object can traverse various status values controlled by the workflow of the associated class. This workflow defines states, transitions, validation rules, and the business logic linked to them. This makes it possible to model and automate complex processes.

Interaction with objects takes place via dynamically generated forms that ensure consistent, validated, and role-based data entry and presentation. The form logic takes into account both the structure of the class and the current object configuration and workflow state. In addition, REST APIs and event-based interfaces are available to integrate external systems and enable automated processes.

The object model in **KleeneStar** combines semantic modeling at the class level with operational data management at the instance level. It thereby creates a robust, rule-based system that ensures both business consistency and technical extensibility.

## Lifecycle and States

Object management in **KleeneStar** is based on a dual lifecycle model that considers both administrative and business aspects. The administrative lifecycle of an object comprises the states "active" and "archived", while the business progression is controlled by the workflow associated with the class.

An object in the "active" state is productively usable and passes through the status values defined in the workflow within this framework, such as "Open", "In Progress", or "Closed". These status values reflect the business dynamics of the object and enable regular operations as well as defined state transitions. The workflow allows flexible modeling of processes without altering the overarching lifecycle.

If an object is no longer needed actively, it can be transferred to the "archived" state. In this mode, the object is write-protected and serves for historization. All data and relationships are retained and remain available for reporting or traceability. If required, an archived object can be restored and returned to the active state.

Additionally, an object can be permanently deleted. This step can be performed from either the active or archived state and is irreversible. The deletion is executed immediately.

The following state diagram illustrates this model and shows the possible transitions between lifecycle states as well as the embedding of the business status logic within the active area:

```
╔══════════════════════════════════════════════════════════════════════════════════════╗
║                            KleeneStar Object State Diagram                           ║
╠══════════════════════════════════════════════════════════════════════════════════════╣
║                                                                                      ║
║                               ┌───────────────────┐                                  ║
║                               │     archive       │                                  ║
║                      new  ╔════════╗         ┌────▼─────┐                            ║
║                        ───► active ║         │ archived │                            ║
║                           ╚══════▲═╝         └─┬──────┬─┘                            ║
║                             │    │   restore   │      │                              ║
║                             │    └─────────────┘      │                              ║
║                             │                         │                              ║
║                             │      ╔═════════╗        │                              ║
║                             └──────► deleted ◄────────┘                              ║
║                                    ╚═════════╝                                       ║
║                                                                                      ║
╚══════════════════════════════════════════════════════════════════════════════════════╝
```

## Data Model

The object data model is a central component of the **KleeneStar** Core Data Models and forms the basis for structured management of business content. Each object is an instance of exactly one class and inherits its field definitions, ensuring a consistent and typed data structure.

The concrete values of an object are stored in so-called `Value` instances. These provide the link between an object and a specific field of its class and enable the storage and validation of data content. Relationships between individual objects (such as links, references, or hierarchies) are represented via `Link` instances, which define both the direction and the type of relationship.

To improve traceability and documentation, objects can be enriched with additional metadata. These include comments (`Comment`) for content explanations, versions (`Version`) for change history, and file references (`FileReference`) to attach external or internal documents. These modular extensions make the data model not only transparent and auditable, but also flexible and extensible for complex application scenarios.

```
╔══════════════════════════════════════════════════════════════════════════════════════╗
║                             KleeneStar Core Data Model                               ║
╠══════════════════════════════════════════════════════════════════════════════════════╣
║                                                                                      ║
║                         ┌────────────────┬────────────────────────────────┐          ║
║                         │ *              │ *                              │          ║
║                   ┌─────▼────┐     ┌─────▼────┐      ┌──────┐ 1           │          ║
║                   │ Workflow │     │ Priority │      │ Form ├───┐         │          ║
║                   └─────┬────┘     └─────┬────┘      └───┬──┘   │         │          ║
║                         │ *              │ *             │ *    │         │          ║
║                         └────────────────┼───────────────┘      │         │          ║
║                                          │                      │         │          ║
║                                          │ 1                    │ *       │          ║
║          ┌───────────┐ *           * ┌───▼───┐ 1          * ┌───▼───┐ 0,1 │          ║
║          │ Workspace ├───────────────► Class ◄──────────────┤ Field ├─────┘          ║
║          └─────┬─────┘               └───▲───┘              └───▲───┘                ║
║                │ 1                       │ 1                    │ 1                  ║
║                └────────────────────┐    │                      │                    ║
║                                     │ *  │ *                    │ *                  ║
║    ┌───────────┐  ┌──────┐ *    2 ┌─▼────┴─┐ 1            * ┌───┴───┐                ║
║    │ Dashboard │  │ Link ├────────► Object ├────────────────► Value │                ║
║    └─────┬─────┘  └──────┘        └─▲────▲─┘                └───▲───┘                ║
║          │ 1                        │ 1  │ 1                    │ 1                  ║
║          │              ┌───────────┘    │                      │                    ║
║          │ *            │ *              │ *                    │ *                  ║
║     ┌────▼───┐     ┌────┴────┐      ┌────┴────┐         ┌───────┴───────┐            ║
║     │ Widget │     │ Comment │      │ Version │         │ FileReference │            ║
║     └────────┘     └─────────┘      └─────────┘         └───────────────┘            ║
║                                                                                      ║
╚══════════════════════════════════════════════════════════════════════════════════════╝
```

## Software Architecture

The architecture of object management follows a modular and decoupled principle. The central control element is the `ObjectManager`, which is responsible for the entire lifecycle and access to all objects within a workspace. It manages object instances, provides a controlled interface for all interactions, and is closely integrated with the `ClassManager`, `FieldManager`, and `WorkflowManager`.

New objects are created exclusively through the `ObjectManager`. This process ensures that the object is correctly initialized, assigned to a class, and given an initial workflow status. For a reactive and loosely coupled architecture, the `ObjectManager` emits events such as `ObjectCreated`, `ObjectUpdated`, and `ObjectDeleted`. Other system components can subscribe to these events to react to changes without being directly dependent on the manager.

The `ObjectManager` handles server-side tasks such as persistent, transactional, and versioned storage of all objects and their values. On every access, it checks the permissions of the calling user or module. Access control is governed by the consolidated permissions derived from workspace, class, and field.

An integrated audit system logs all relevant actions around objects: accesses, value changes, status transitions, and permission checks are recorded with timestamp, user identity, object ID, and action type. This data supports analytics, troubleshooting, compliance checks, and state restoration.

```
╔KleeneStar.Core═══════════════════════════════════════════════════════════════════════╗
║                                                                                      ║
║                              ┌────────────────────┐                                  ║
║                              │ <<Interface>>      │                                  ║
║                              │ IComponentManager  │                                  ║
║                              ├────────────────────┤                                  ║
║                              └────────Δ───────────┘                                  ║
║                                       ¦                                              ║
║                                       ¦                                              ║
║                     ┌─────────────────┴─────────────────────┐                        ║
║                     │ <<Interface>>                         │                        ║
║         ┌-----------┤ IObjectManager                        │                        ║
║         ¦           ├───────────────────────────────────────┤                        ║
║         ¦           │ ObjectCreated:Event                   │                        ║
║         ¦           │ ObjectUpdated:Event                   │                        ║
║         ¦           │ ObjectDeleted:Event                   │                        ║
║         ¦           │ ObjectTransitioned:Event              │                        ║
║         ¦           ├───────────────────────────────────────┤ 1                      ║
║         ¦           │ GetObject(key):IObject                ├───────┐                ║
║         ¦           │ GetObjects(query):IEnumerable<IObject>│       │                ║
║         ¦           │ CreateObject(class, values):IObject   │       │                ║
║         ¦           │ UpdateObject(key, values):IObject     │       │                ║
║         ¦           │ DeleteObject(key):void                │       │                ║
║         ¦           │ TransitionObject(key, transition):    │       │                ║
║         ¦           │   IObject                             │       │                ║
║         ¦           └───────────────────────────────────────┘       │                ║
║         ¦                                                           │                ║
║         ¦                                                           │                ║
║         ¦                            ┌───────────────┐              │                ║
║         ¦                            │ <<Interface>> │              │                ║
║         ¦                            │ IModel        │              │                ║
║         ¦                            ├───────────────┤              │                ║
║         ¦                            └───────Δ───────┘              │                ║
║         ¦                                    ¦                      │                ║
║         ¦                                    ¦                      │                ║
║         ¦               ┌────────────────────┴───────────────┐ *    │                ║
║         ¦               │ <<Interface>>                      ◄──────┘                ║
║         ¦               │ IObject                            │                       ║
║         ¦               ├────────────────────────────────────┤                       ║
║         ¦               │ Id:Guid                            │                       ║
║         ¦               │ Key:String                         │                       ║
║         ¦               │ Summary:String                     │                       ║
║         ¦               │ Description:String                 │                       ║
║         ¦               │ Class:IClass                       │                       ║
║         ¦               │ Workspace:IWorkspace               │                       ║
║         ¦               │ State:IWorkflowState               │ 1                     ║
║         ¦               │ Values:IEnumerable<IValue>         ├─────────┐             ║
║         ¦               │ Created:DateTime                   │         │             ║
║         ¦               │ Updated:DateTime                   │         │             ║
║         ¦               │ PermissionsProfiles:               │         │             ║
║         ¦               │   IEnumerable<IPermissionsProfile> │         │             ║
║         ¦               └──────────────────Δ─────────────────┘         │             ║
║         ¦                                  ¦                           │ *           ║
║         ¦                                  ¦                   ┌───────▼───────┐     ║
║         ¦                                  ¦                   │ <<Interface>> │     ║
║         ¦                                  ¦                   │ IValue        │     ║
║         ¦                                  ¦                   ├───────────────┤     ║
║         ¦                                  ¦                   │ Id:Guid       │     ║
║         ¦                                  ¦                   │ Field:IField  │     ║
║         ¦                                  ¦                   │ Value:Object  │     ║
║         ¦                                  ¦                   └───────────────┘     ║
║         ¦ create        ┌──────────────────┴─────────────────┐                       ║
║         └---------------► Object                             │                       ║
║                         ├────────────────────────────────────┤                       ║
║                         │ Id:Guid                            │                       ║
║                         │ Key:String                         │                       ║
║                         │ Summary:String                     │                       ║
║                         │ Description:String                 │                       ║
║                         │ Class:IClass                       │                       ║
║                         │ Workspace:IWorkspace               │                       ║
║                         │ State:IWorkflowState               │                       ║
║                         │ Values:IEnumerable<Value>          │                       ║
║                         │ Created:DateTime                   │                       ║
║                         │ Updated:DateTime                   │                       ║
║                         │ PermissionsProfiles:               │                       ║
║                         │   IEnumerable<IPermissionsProfile> │                       ║
║                         └────────────────────────────────────┘                       ║
║                                                                                      ║
╚══════════════════════════════════════════════════════════════════════════════════════╝
```

The **KleeneStar**.Core model describes a modular, object-oriented architecture for managing structured data objects within a systematic and extensible framework. It is based on the principle of separation of concerns and clear definition of interfaces for key functions such as object management, data modeling, and access control.

At the center is the `IComponentManager` interface, which serves as the overarching management unit. Derived from it is `IObjectManager`, a specialized interface that provides all essential operations for object lifecycle management. These include creating, fetching, updating, deleting, and transitioning objects. These operations are event-driven, meaning that corresponding events are triggered on each change, such as `ObjectCreated` or `ObjectUpdated`.

Objects themselves are described by the `IObject` interface. In addition to a unique key, they contain metadata such as class, workspace, and current state. The content data of an object is organized as `IValue` instances, each representing a field and its value. Additionally, objects can be equipped with permission profiles (`IPermissionsProfile`) to define granular access rights.

The architecture also foresees a clear separation between interfaces and implementations. `IObject` is implemented by the concrete object class, which provides all defined properties and methods. This separation enables high flexibility and extensibility of the system.

## UI Concepts and Pages

The **KleeneStar** WebApp user interface translates the abstract data models and lifecycle rules of object management into a concrete and user-friendly application. The goal is to enable intuitive, efficient, and secure interaction with structured objects and their metadata.

The design follows the established UI patterns of the **KleeneStar** WebApp. This creates strong familiarity, easing onboarding and significantly shortening the learning curve. The planned UI mockups serve as visual drafts for later implementation. They define:

- navigation within the application,
- the arrangement of controls,
- the presentation of different system states.

They illustrate how users are guided through the application to perform common tasks, such as creating new objects, viewing and editing existing entries, performing workflow transitions, or deleting objects.

### Object Management - Object Search (Page)

The global search feature, accessible via the main navigation, is a central tool for fast and precise information retrieval. It enables full-text search across all objects and fields that are visible to the user according to their permissions. The search is workspace-wide, meaning it spans all workspaces to which the user has access, and delivers consolidated results independent of the currently open context. In addition, precise filter criteria such as class, status, priority, or assignee can be combined to further narrow down the results.

The search page offers an interactive interface where queries can be formulated directly and results are displayed immediately. The search syntax is based on WQL (WebExpress Query Language) and can be saved and reused if needed. The result list shows a compact overview of matches, including status and relevance, and allows direct navigation to the respective object detail.

The search supports both temporary filters (e.g., "Recently updated") and user-defined favorites, making frequently used queries quickly accessible. Actions such as "Edit", "Clone", or "Delete" are available directly from the result list, depending on the user’s permissions.

```
╔WebAppPage════════════════════════════════════════════════════════════════════════════╗
║┌Header──────────────────────────────────────────────────────────────────────────────┐║
║│ * KleeneStar     Workspace ▼   Dashboard ▼       [+ AddObject]         [Search]    │║
║└────────────────────────────────────────────────────────────────────────────────────┘║
║┌Breadcrumb──────────────────────────────────────────────────────────────────────────┐║
║│ / Search                                                                           │║
║└────────────────────────────────────────────────────────────────────────────────────┘║
║┌Filter────────────────┐ ┌Search Content─────────────────────────────────────────────┐║
║│             [Search] │░│                                                           │║
║│ Filters:             │░│ WQL: [ Text~"vpn-gateway" and Assignee="Max Power"][Save] │║
║│ - All                │░│                                                           │║
║│ - Active             │░│ Summary                           | Status | Impact     + │║
║│ - Archived           │░│-----------------------------------|--------|--------------│║
║│ - My Objects         │░│ VPN connection disrupted          | Open   | High     […] │║
║│ - Created recently   │░│ ▼ VPN Access for new employee     | Done   | Medium    ¦  │║
║│ - Updated recently   │░│  - AD account created for new empl| Done  ┌────────────┴┐ │║
║│ - Viewed recently    │░│  - VPN credentials sent via email | Done  │ Edit        │ │║
║│                      │░│                                           │ Clone       │ │║
║│ Favorites:           │<│                                   ‹ Prev  │ Permissions │ │║
║│ - My Tasks           │<│                                           │ <section>   │ │║
║│ - New                │<│                                           ├─────────────┤ │║
║│ - In Progress        │░│                                           │ Delete      │ │║
║│ - Resolved           │░│                                           └─────────────┘ │║
║│                      │░│                                                           │║
║│ My Filters:          │░│                                                           │║
║│ - All Tasks          │░│                                                           │║
║│ - My Tasks           │░│                                                           │║
║├──────────────────────┤░│                                                           │║
║│ [+] | [Settings]  << │░│                                                           │║
║└──────────────────────┘ └───────────────────────────────────────────────────────────┘║
║┌Footer──────────────────────────────────────────────────────────────────────────────┐║
║│ [Documentation]        |        KleeneStar v1.2.3        |      [Report a problem] │║
║└────────────────────────────────────────────────────────────────────────────────────┘║
╚══════════════════════════════════════════════════════════════════════════════════════╝
```

### Object Management - Object Overview (Page)

The object overview page is the central view for displaying objects of a specific class within a workspace. It provides a tabular or card-based list that can be searched, filtered, and sorted. Each object is displayed with its most important attributes. New objects can be created via the "+ Add Object" button. Context menus on each object provide direct access to actions such as editing, deleting, or performing workflow transitions.

```
╔WebAppPage════════════════════════════════════════════════════════════════════════════╗
║┌Header──────────────────────────────────────────────────────────────────────────────┐║
║│ * KleeneStar     Workspace ▼   Dashboard ▼       [+ AddObject]         [Search]    │║
║└────────────────────────────────────────────────────────────────────────────────────┘║
║┌Breadcrumb──────────────────────────────────────────────────────────────────────────┐║
║│ / Service Desk                                                                     │║
║└────────────────────────────────────────────────────────────────────────────────────┘║
║┌Workspace─────────────┐ ┌Object Content─────────────────────────────────────────────┐║
║│[Name]                │░│                                                           │║
║│                      │░│ Incident                                              […] │║
║│      [Icon]          │░│                                                  [Search] │║
║│                      │░│ Summary                          | Status  | Impact     + │║
║│           [ Search ] │░│----------------------------------|---------|--------------│║
║│ Issue                │░│ VPN connection disrupted         | Open    | High     […] │║
║│ ├─ Incident          │░│ Outlook won't start              | Open    | Medium    ¦  │║
║│ ├─ Problem           │░│ Printer on floor 3 offline       | Assigne┌────────────┴┐ │║
║│ └─ ServiceRequest    │░│ File upload fails                | In Prog│ Edit        │ │║
║│                      │░│ Remote desktop not reachable     | Open   │ Clone       │ │║
║│                      │<│ Password reset not possible      | Closed │ Permissions │ │║
║│                      │<│ Wi-Fi outage in conference room  | Open   │ <section>   │ │║
║│                      │<│ Teams notifications delayed      | Assigne├─────────────┤ │║
║│                      │░│ Scanner not sending PDFs         | Assigne│ Delete      │ │║
║│                      │░│ SharePoint access denied         | Open   └─────────────┘ │║
║│                      │░│ Software update blocks startup   | In Prog.| High     […] │║
║│                      │░│ Screen flickers intermittently   | Closed  | Medium   […] │║
║│                      │░│                                                           │║
║│                      │░│                                   ‹ Prev  1  2  3  Next › │║
║├──────────────────────┤░│                                                           │║
║│ [+] | [Settings]  << │░│                                                           │║
║└──────────────────────┘ └───────────────────────────────────────────────────────────┘║
║┌Footer──────────────────────────────────────────────────────────────────────────────┐║
║│ [Documentation]        |        KleeneStar v1.2.3        |      [Report a problem] │║
║└────────────────────────────────────────────────────────────────────────────────────┘║
╚══════════════════════════════════════════════════════════════════════════════════════╝
```

### Object Management - Object Detail View (Page)

The detail view of an object is presented via a form defined in the form management for the respective object class. This view displays all fields and metadata of the object in a structured form and provides a comprehensive overview of the current state and context. The page is typically divided into sections to present different information areas such as attributes, linked objects, comments, attachments, and version history clearly.

Workflow transitions are prominently offered as actions, enabling the user to advance the process directly from the detail view. These actions are context-sensitive and may offer different options depending on the status and role.

A central component of the detail view is the comment area. Users can add new comments, reply to existing posts, and build a chronological communication history for the object. This supports transparent documentation of decisions, inquiries, and processing steps.

Handling attachments is also user-friendly: files can be dragged and dropped directly into the window, where they are automatically associated with the object and displayed in the "Attachments" section. This makes it easy to quickly add screenshots, documents, or other relevant files without needing separate upload steps.

```
╔WebAppPage════════════════════════════════════════════════════════════════════════════╗
║┌Header──────────────────────────────────────────────────────────────────────────────┐║
║│ * KleeneStar     Workspace ▼   Dashboard ▼       [+ AddObject]         [Search]    │║
║└────────────────────────────────────────────────────────────────────────────────────┘║
║┌Breadcrumb──────────────────────────────────────────────────────────────────────────┐║
║│ / Service Desk / INC-00123                                                         │║
║└────────────────────────────────────────────────────────────────────────────────────┘║
║┌Workspace─────────────┐ ┌Object Content────────────────────────────┬────────────────┐║
║│[Name]                │░│                                          │                │║
║│                      │░│ INC-00123: VPN connection disrupted  […] │   Status: Open▼│║
║│      [Icon]          │░│                                       ¦  │ Priority: High │║
║│                      │░│ Description: Users cann┌──────────────┴┐.│ Assignee: Max  │║
║│           [ Search ] │░│ Affected CI: vpn-gatewa│ Edit          │ │           Power│║
║│ Issue                │░│ ...                    │ Clone         │ │  Created: 2025 │║
║│ ├─ Incident          │░│                        │ Add Link      │ │           -01  │║
║│ ├─ Problem           │░│ ⠿ Attachments:         │ Add Subobject │ │           -16  │║
║│ └─ ServiceRequest    │░│   - Screenshot.png     │ Show as...    │ │                │║
║│                      │░│                        │ Move          │ │  Watchers:  [+]│║
║│                      │<│                        │ Export        │ │   - Erika Mus x│║
║│                      │<│ ⠿ Comments:            │ Permissions   │ │                │║
║│                      │<│   - Max Power (2025-01-│ <section>     │ │  Link:      [+]│║
║│                      │░│       Can you please ch├───────────────┤ │  - INC-00321  x│║
║│                      │░│       status?          │ Delete        │ │                │║
║│                      │░│                        └───────────────┘ │                │║
║│                      │░│   - Erika Mustermann (2025-01-16 09:15)  │                │║
║│                      │░│       I have checked the logs, no        │                │║
║│                      │░│       errors found.                      │                │║
║├──────────────────────┤░│                                          │                │║
║│ [+] | [Settings]  << │░│ [ Add new comment...                   ] │                │║
║└──────────────────────┘ └──────────────────────────────────────────┴────────────────┘║
║┌Footer──────────────────────────────────────────────────────────────────────────────┐║
║│ [Documentation]        |        KleeneStar v1.2.3        |      [Report a problem] │║
║└────────────────────────────────────────────────────────────────────────────────────┘║
╚══════════════════════════════════════════════════════════════════════════════════════╝
```

### Object Management - Templates (Modal)

Clicking the "+ Add Object" button opens a central modal that offers the user a selection of available object templates. Each template is visually represented as a card and contains the class name, a short description, and an icon for easier recognition. Selecting a template initializes a new object with predefined fields and settings.

```
╔WebAppPage════════════════════════════════════════════════════════════════════════════╗
║┌Header──────────────────────────────────────────────────────────────────────────────┐║
║│ * KleeneStar     Workspace ▼   Dashboard ▼       [+ AddObject]         [Search]    │║
║└─────╔ObjectBlueprintModal════════════════════════════════════════════════════╗─────┘║
║┌Bread║┌Form──────────────────────────────────────────────────────────────────┐║─────┐║
║│ / Se║│ Create new object                                                    │║     │║
║└─────║├──────────────┬───────────────────────────────────────────────────────┤║─────┘║
║┌Works║│              │ Select an object template:                   [Search] │║─────┐║
║│[Name║│ - All        │                                                       │║     │║
║│     ║│ - Category 0 │ ┌BlueprintGrid──────────────────────────────────────┐ │║ […] │║
║│     ║│ - Category 1 │ │                                                  ▲│ │║rch] │║
║│     ║│ - ...        │ │ ┌───────────────────┐  ┌────────────────────┐    ▒│ │║     │║
║│     ║│ - Category n │ │ │ [Icon] Incident   │  │ [Icon] Problem     │    ░│ │║-----│║
║│ Issu║│              │ │ │        Service    │  │        Technical   │    ░│ │║ […] │║
║│ ├─ I║│              │ │ │        disruption │  │        defect      │    ░│ │║ […] │║
║│ ├─ P║│              │ │ └───────────────────┘  └────────────────────┘    ░│ │║ […] │║
║│ └─ S║│              │ │                                                  ░│ │║ […] │║
║│     ║│              │ │ ┌───────────────────┐  ┌────────────────────┐    ░│ │║ […] │║
║│     ║│              │ │ │ [Icon] ServiceReq │  │ [Icon] Change      │    ░│ │║ […] │║
║│     ║│              │ │ │        Request    │  │        Change      │    ░│ │║ […] │║
║│     ║│              │ │ │        by user    │  │        request     │    ░│ │║ […] │║
║│     ║│              │ │ └───────────────────┘  └────────────────────┘    ░│ │║ […] │║
║│     ║│              │ │                                                  ░│ │║ […] │║
║│     ║│              │ │ ┌───────────────────┐  ┌────────────────────┐    ░│ │║ […] │║
║│     ║│              │ │ │ [Icon] Release    │  │ [Icon] Task        │    ▼│ │║ […] │║
║│     ║│              │ └───────────────────────────────────────────────────┘ │║     │║
║│     ║└──────────────┴───────────────────────────────────────────────────────┘║xt › │║
║├─────║                                                                        ║     │║
║│ [+] ║                                                       [Next] [Cancel]  ║     │║
║└─────║                                                                        ║─────┘║
║┌Foote╚════════════════════════════════════════════════════════════════════════╝─────┐║
║│ [Documentation]        |        KleeneStar v1.2.3        |      [Report a problem] │║
║└────────────────────────────────────────────────────────────────────────────────────┘║
╚══════════════════════════════════════════════════════════════════════════════════════╝
```

### Object Management - New/Edit (Modal)

Creating and editing objects is done via forms displayed in a modal dialog. The displayed form is determined by the class and context (create, edit, or workflow transition) and dynamically loaded from the `FormManager`. It mirrors exactly the structure defined in the form designer, including all fields, their arrangement, and any layout groups (e.g., horizontal or column-based sections). All inputs are validated server-side based on field and workflow rules before the object is saved.

```
╔WebAppPage════════════════════════════════════════════════════════════════════════════╗
║┌Header──────────────────────────────────────────────────────────────────────────────┐║
║│ * KleeneStar     Workspace ▼   Dashboard ▼       [+ AddObject]         [Search]    │║
║└─────╔ObjectAddEditModal══════════════════════════════════════════════════════╗─────┘║
║┌Bread║┌Form──────────────────────────────────────────────────────────────────┐║─────┐║
║│ / Se║│ Add Incident / Edit Incident                                         │║     │║
║└─────║├──────────────────────────────────────────────────────────────────────┤║─────┘║
║┌Works║│ ┌───────┬───────┬─────┬───────┬────────────────────────────────────┐ │║─────┐║
║│[Name║│ │ Tab 1 │ Tab 2 │ ... | Tab n |                                    │ │║     │║
║│     ║│ │       └───────┴─────┴───────┴────────────────────────────────────┤ │║ […] │║
║│     ║│ │                                                                  │ │║rch] │║
║│     ║│ │     Summary*: [                                                ] │ │║     │║
║│     ║│ │       Status: [ Open                                          ▼] │ │║-----│║
║│ Issu║│ │     Priority: [ Medium                                        ▼] │ │║ […] │║
║│ ├─ I║│ │     Assignee: [                                               ▼] │ │║ […] │║
║│ ├─ P║│ │         Tags: [                                                ] │ │║ […] │║
║│ └─ S║│ │  Description: [                                                ] │ │║ […] │║
║│     ║│ │ Reported At*: [ 2025-11-07 15:00]     Affected CI: [          ▼] │ │║ […] │║
║│     ║│ │                                                                  │ │║ […] │║
║│     ║│ │                                                                  │ │║ […] │║
║│     ║│ │                                                                  │ │║ […] │║
║│     ║│ │                                                                  │ │║ […] │║
║│     ║│ │                                                                  │ │║ […] │║
║│     ║│ │                                                                  │ │║ […] │║
║│     ║│ │                                                                  │ │║ […] │║
║│     ║│ └──────────────────────────────────────────────────────────────────┘ │║     │║
║│     ║└──────────────────────────────────────────────────────────────────────┘║xt › │║
║├─────║                                                                        ║     │║
║│ [+] ║                                                       [Save] [Cancel]  ║     │║
║└─────║                                                                        ║─────┘║
║┌Foote╚════════════════════════════════════════════════════════════════════════╝─────┐║
║│ [Documentation]        |        KleeneStar v1.2.3        |      [Report a problem] │║
║└────────────────────────────────────────────────────────────────────────────────────┘║
╚══════════════════════════════════════════════════════════════════════════════════════╝
```

### Object Management - Add Link (Modal)

The "Add Link" function enables linking external or internal references directly to an object. It is available within the object view and serves to present relevant information, resources, or relationships clearly and traceably.

Via the "Add Link" button, a user can enter a URL or a system-internal reference. This facilitates orientation and supports collaboration by linking to related issues, documentation, knowledge articles, or external tools, for example. All added links are displayed in the object context and are directly accessible to authorized users. Link management is dynamic. Links can be added, edited, or removed at any time. The function thus contributes significantly to transparency and structured information networking within KleeneStar.

#### Recently viewed

This area displays recently viewed objects that are particularly suitable for quick linking. The list provides a compact overview of titles and statuses of recently viewed entries. This way, frequently used or currently relevant content can be linked directly to the object without lengthy searches.

The search function also enables targeted finding of a specific entry within the recently viewed objects.

```
╔WebAppPage════════════════════════════════════════════════════════════════════════════╗
║┌Header──────────────────────────────────────────────────────────────────────────────┐║
║│ * KleeneStar     Workspace ▼   Dashboard ▼       [+ AddObject]         [Search]    │║
║└─────╔ObjectLinkModal═════════════════════════════════════════════════════════╗─────┘║
║┌Bread║┌Form──────────────────────────────────────────────────────────────────┐║─────┐║
║│ / Se║│ Add link                                                             │║     │║
║└─────║├────────────────────┬─────────────────────────────────────────────────┤║─────┘║
║┌Works║│                    │ Recently viewed                        [Search] │║─────┐║
║│[Name║│ - Recently viewed  │                                                 │║     │║
║│     ║│ - Recently created │ Summary                        | Status       + │║ […] │║
║│     ║│ - Object Link      │--------------------------------|----------------│║rch] │║
║│     ║│ - Web Link         │ Scanner not sending PDFs       | Assigne        │║     │║
║│ Issu║│                    │ Screen flickers intermittently | Closed         │║-----│║
║│ ├─ I║│                    │ File upload fails              | In Prog.       │║ […] │║
║│ ├─ P║│                    │                                                 │║ […] │║
║│ └─ S║│                    │                         ‹ Prev  1  2  3  Next › │║ […] │║
║│     ║│                    │                                                 │║ […] │║
║│     ║│                    │                                                 │║ […] │║
║│     ║│                    │                                                 │║ […] │║
║│     ║│                    │                                                 │║ […] │║
║│     ║│                    │                                                 │║ […] │║
║│     ║│                    │                                                 │║ […] │║
║│     ║│                    │                                                 │║ […] │║
║│     ║│                    │                                                 │║     │║
║│     ║└────────────────────┴─────────────────────────────────────────────────┘║xt › │║
║├─────║                                                                        ║     │║
║│ [+] ║                                                        [Add] [Cancel]  ║     │║
║└─────║                                                                        ║─────┘║
║┌Foote╚════════════════════════════════════════════════════════════════════════╝─────┐║
║│ [Documentation]        |        KleeneStar v1.2.3        |      [Report a problem] │║
║└────────────────────────────────────────────────────────────────────────────────────┘║
╚══════════════════════════════════════════════════════════════════════════════════════╝
```

#### Recently created

This area displays recently created objects, for example new incidents, change requests, or service requests. It is especially useful when linking to a newly created item, for documentation or tracking purposes.

Titles and status are provided for quick orientation. The integrated search function helps filter for a specific object and link it directly.

```
╔WebAppPage════════════════════════════════════════════════════════════════════════════╗
║┌Header──────────────────────────────────────────────────────────────────────────────┐║
║│ * KleeneStar     Workspace ▼   Dashboard ▼       [+ AddObject]         [Search]    │║
║└─────╔ObjectLinkModal═════════════════════════════════════════════════════════╗─────┘║
║┌Bread║┌Form──────────────────────────────────────────────────────────────────┐║─────┐║
║│ / Se║│ Add link                                                             │║     │║
║└─────║├────────────────────┬─────────────────────────────────────────────────┤║─────┘║
║┌Works║│                    │ Recently created                       [Search] │║─────┐║
║│[Name║│ - Recently viewed  │                                                 │║     │║
║│     ║│ - Recently created │ Summary                         | Status      + │║ […] │║
║│     ║│ - Object Link      │---------------------------------|---------------│║rch] │║
║│     ║│ - Web Link         │ SharePoint access denied        | Open          │║     │║
║│     ║│                    │ VPN connection disrupted        | Open          │║-----│║
║│ Issu║│                    │ Outlook won't start             | Open          │║ […] │║
║│ ├─ I║│                    │ Remote desktop not reachable    | Open          │║ […] │║
║│ ├─ P║│                    │ Wi-Fi outage in conference room | Open          │║ […] │║
║│ └─ S║│                    │                                                 │║ […] │║
║│     ║│                    │                         ‹ Prev  1  2  3  Next › │║ […] │║
║│     ║│                    │                                                 │║ […] │║
║│     ║│                    │                                                 │║ […] │║
║│     ║│                    │                                                 │║ […] │║
║│     ║│                    │                                                 │║ […] │║
║│     ║│                    │                                                 │║ […] │║
║│     ║│                    │                                                 │║ […] │║
║│     ║│                    │                                                 │║     │║
║│     ║└────────────────────┴─────────────────────────────────────────────────┘║xt › │║
║├─────║                                                                        ║     │║
║│ [+] ║                                                        [Add] [Cancel]  ║     │║
║└─────║                                                                        ║─────┘║
║┌Foote╚════════════════════════════════════════════════════════════════════════╝─────┐║
║│ [Documentation]        |        KleeneStar v1.2.3        |      [Report a problem] │║
║└────────────────────────────────────────────────────────────────────────────────────┘║
╚══════════════════════════════════════════════════════════════════════════════════════╝
```

#### Object Link

In this area, internal system objects can be linked directly with each other. Selection is made via a query or filtering, for example by class types or responsibilities. This allows related tickets, tasks, or configuration items to be purposefully connected to transparently represent dependencies or relationships.

The displayed list is based on the defined search query (WQL) and shows relevant objects with title and status. The linking facilitates navigation and improves traceability of complex processes within KleeneStar.

```
╔WebAppPage════════════════════════════════════════════════════════════════════════════╗
║┌Header──────────────────────────────────────────────────────────────────────────────┐║
║│ * KleeneStar     Workspace ▼   Dashboard ▼       [+ AddObject]         [Search]    │║
║└─────╔ObjectLinkModal═════════════════════════════════════════════════════════╗─────┘║
║┌Bread║┌Form──────────────────────────────────────────────────────────────────┐║─────┐║
║│ / Se║│ Add link                                                             │║     │║
║└─────║├────────────────────┬─────────────────────────────────────────────────┤║─────┘║
║┌Works║│                    │ Object Link                                     │║─────┐║
║│[Name║│ - Recently viewed  │                                                 │║     │║
║│     ║│ - Recently created │ WQL: [class=incident and assignee='Max Power']  │║ […] │║
║│     ║│ - Object Link      │                                                 │║rch] │║
║│     ║│ - Web Link         │ Summary                     | Status          + │║     │║
║│     ║│                    │-----------------------------|-------------------│║-----│║
║│ Issu║│                    │ Printer on floor 3 offline  | Assigne           │║ […] │║
║│ ├─ I║│                    │ Teams notifications delayed | Assigne           │║ […] │║
║│ ├─ P║│                    │ Scanner not sending PDFs    | Assigne           │║ […] │║
║│ └─ S║│                    │                                                 │║ […] │║
║│     ║│                    │                         ‹ Prev  1  2  3  Next › │║ […] │║
║│     ║│                    │                                                 │║ […] │║
║│     ║│                    │                                                 │║ […] │║
║│     ║│                    │                                                 │║ […] │║
║│     ║│                    │                                                 │║ […] │║
║│     ║│                    │                                                 │║ […] │║
║│     ║│                    │                                                 │║ […] │║
║│     ║│                    │                                                 │║     │║
║│     ║└────────────────────┴─────────────────────────────────────────────────┘║xt › │║
║├─────║                                                                        ║     │║
║│ [+] ║                                                        [Add] [Cancel]  ║     │║
║└─────║                                                                        ║─────┘║
║┌Foote╚════════════════════════════════════════════════════════════════════════╝─────┐║
║│ [Documentation]        |        KleeneStar v1.2.3        |      [Report a problem] │║
║└────────────────────────────────────────────────────────────────────────────────────┘║
╚══════════════════════════════════════════════════════════════════════════════════════╝
```

#### Web Link

External resources such as websites, documentation, or tools can be linked to the object here. The user enters a URL and can optionally add a title to clarify the context.

This function is particularly suitable for integrating knowledge articles, support portals, or external systems. Links are directly accessible to authorized users and can be edited or removed at any time. This creates structured information networking across system boundaries.

```
╔WebAppPage════════════════════════════════════════════════════════════════════════════╗
║┌Header──────────────────────────────────────────────────────────────────────────────┐║
║│ * KleeneStar     Workspace ▼   Dashboard ▼       [+ AddObject]         [Search]    │║
║└─────╔ObjectLinkModal═════════════════════════════════════════════════════════╗─────┘║
║┌Bread║┌Form──────────────────────────────────────────────────────────────────┐║─────┐║
║│ / Se║│ Add link                                                             │║     │║
║└─────║├────────────────────┬─────────────────────────────────────────────────┤║─────┘║
║┌Works║│                    │ Web Link                                        │║─────┐║
║│[Name║│ - Recently viewed  │                                                 │║     │║
║│     ║│ - Recently created │  Title: [ API documentation                   ] │║ […] │║
║│     ║│ - Object           │   URL*: [ https://example.com/api-docs        ] │║rch] │║
║│     ║│ - Web Link         │                                                 │║     │║
║│     ║│                    │                                                 │║-----│║
║│ Issu║│                    │                                                 │║ […] │║
║│ ├─ I║│                    │                                                 │║ […] │║
║│ ├─ P║│                    │                                                 │║ […] │║
║│ └─ S║│                    │                                                 │║ […] │║
║│     ║│                    │                                                 │║ […] │║
║│     ║│                    │                                                 │║ […] │║
║│     ║│                    │                                                 │║ […] │║
║│     ║│                    │                                                 │║ […] │║
║│     ║│                    │                                                 │║ […] │║
║│     ║│                    │                                                 │║ […] │║
║│     ║│                    │                                                 │║ […] │║
║│     ║│                    │                                                 │║ […] │║
║│     ║│                    │                                                 │║     │║
║│     ║└────────────────────┴─────────────────────────────────────────────────┘║xt › │║
║├─────║                                                                        ║     │║
║│ [+] ║                                                        [Add] [Cancel]  ║     │║
║└─────║                                                                        ║─────┘║
║┌Foote╚════════════════════════════════════════════════════════════════════════╝─────┐║
║│ [Documentation]        |        KleeneStar v1.2.3        |      [Report a problem] │║
║└────────────────────────────────────────────────────────────────────────────────────┘║
╚══════════════════════════════════════════════════════════════════════════════════════╝
```

### Object Management - Object Transition with Form (Modal)

A modal dialog is used to perform a workflow transition. It serves the structured collection of information required for the respective transition (for example a justification, result, or additional details).

The form displayed within the modal is based on the transition form assigned to the transition. This defines which fields are displayed, whether input is mandatory, and which validations apply. This functionality is only available if a form is assigned to the transition. If no form is assigned, no dialog is shown, and the transition is performed directly.

```
╔WebAppPage════════════════════════════════════════════════════════════════════════════╗
║┌Header──────────────────────────────────────────────────────────────────────────────┐║
║│ * KleeneStar     Workspace ▼   Dashboard ▼       [+ AddObject]         [Search]    │║
║└─────╔ObjectTransitionModal═══════════════════════════════════════════════════╗─────┘║
║┌Bread║┌Form──────────────────────────────────────────────────────────────────┐║─────┐║
║│ / Se║│ Execute Transition: Resolve                                          │║     │║
║└─────║├──────────────────────────────────────────────────────────────────────┤║─────┘║
║┌Works║│ The object will be moved to status 'Done'.                           │║─────┐║
║│[Name║│ ┌───────┬──────────────────────────────────────────────────────────┐ │║     │║
║│     ║│ │ Tab 1 │                                                          │ │║ […] │║
║│     ║│ │       └──────────────────────────────────────────────────────────┤ │║rch] │║
║│     ║│ │                                                                  │ │║     │║
║│     ║│ │  Resolution*: [ Done                                          ▼] │ │║-----│║
║│ Issu║│ │     Assignee: [                                               ▼] │ │║ […] │║
║│ ├─ I║│ │  Description: [ Issue resolved by restarting the service.      ] │ │║ […] │║
║│ ├─ P║│ │                                                                  │ │║ […] │║
║│ └─ S║│ │                                                                  │ │║ […] │║
║│     ║│ │                                                                  │ │║ […] │║
║│     ║│ │                                                                  │ │║ […] │║
║│     ║│ │                                                                  │ │║ […] │║
║│     ║│ │                                                                  │ │║ […] │║
║│     ║│ │                                                                  │ │║ […] │║
║│     ║│ │                                                                  │ │║ […] │║
║│     ║│ │                                                                  │ │║ […] │║
║│     ║│ │                                                                  │ │║ […] │║
║│     ║│ └──────────────────────────────────────────────────────────────────┘ │║     │║
║│     ║└──────────────────────────────────────────────────────────────────────┘║xt › │║
║├─────║                                                                        ║     │║
║│ [+] ║                                                    [Resolve] [Cancel]  ║     │║
║└─────║                                                                        ║─────┘║
║┌Foote╚════════════════════════════════════════════════════════════════════════╝─────┐║
║│ [Documentation]        |        KleeneStar v1.2.3        |      [Report a problem] │║
║└────────────────────────────────────────────────────────────────────────────────────┘║
╚══════════════════════════════════════════════════════════════════════════════════════╝
```

### Object Management - Add Subobject (Modal)

The "Add Subobject" function enables creating a new subobject that is directly linked to the currently opened object. This connection is established via a parent link, where the main object is referenced as the parent element in the newly created subobject.

The subobject is created in a defined target class, typically intended for detailed information, subprocesses, or dependent tasks. The parent link ensures a clear relationship between main and subobject (both technically and visually). In the interface, this relationship is represented through appropriate navigation paths, links, or embedded views.

"Add Subobject" is especially helpful when complex situations need to be structured, for example incidents with associated measures, projects with subtasks, or requests with follow-up processes.

When triggering the function, the template page is displayed first, analogous to the regular object creation process. This serves to select a suitable object class for the new subobject. The selection is filtered so that only classes allowed by the `AllowedChildren` property of the current class and permitted for the user are shown. Only after selecting an allowed class is the input form opened.

The input form for the new subobject corresponds to regular object creation. However, some fields are already inherited and prefilled from the parent object to ensure consistency and efficiency in data capture.

### Object Management - Show as (Modal)

The "Show as…" function makes it possible to view the user interface from the perspective of another person. It is used to check the effect of the permission system and to understand which information is visible to a particular role or group and which is hidden.

When "Show as…" is activated, the view is adjusted as if logged in with the selected user profile. This allows differences in object display, field visibility, and action availability to be seen directly. This function is particularly helpful for administrators to test permissions, validate roles, or analyze support requests.

```
╔WebAppPage════════════════════════════════════════════════════════════════════════════╗
║┌Header──────────────────────────────────────────────────────────────────────────────┐║
║│ * KleeneStar     Workspace ▼   Dashboard ▼       [+ AddObject]         [Search]    │║
║└─────╔ObjectShowAsModal═══════════════════════════════════════════════════════╗─────┘║
║┌Bread║┌Form──────────────────────────────────────────────────────────────────┐║─────┐║
║│ / Se║│ Show as...                                                           │║     │║
║└─────║├──────────────────────────────────────────────────────────────────────┤║─────┘║
║┌Works║│ Select a person or role whose perspective you want to adopt.         │║─────┐║
║│[Name║│ The view will be adjusted according to that person's permissions.    │║     │║
║│     ║│                                                                      │║ […] │║
║│     ║│                                                             [Search] │║     │║
║│     ║│  ▢ Max Mustermann                                                    │║-----│║
║│ Issu║│  ▢ Anna Becker                                                       │║ […] │║
║│ ├─ I║│  ▢ Jonas Richter                                                     │║ […] │║
║│ ├─ P║│  ▢ Lisa Sommer                                                       │║ […] │║
║│ └─ S║│  ▢ Tom Neumann                                                       │║ […] │║
║│     ║│  ▢ Guest user                                                        │║ […] │║
║│     ║│                                                                      │║ […] │║
║│     ║│                                                                      │║ […] │║
║│     ║│                                                                      │║ […] │║
║│     ║│                                                                      │║ […] │║
║│     ║│                                                                      │║ […] │║
║│     ║│                                                                      │║ […] │║
║│     ║│                                                                      │║     │║
║│     ║└──────────────────────────────────────────────────────────────────────┘║xt › │║
║├─────║                                                                        ║     │║
║│ [+] ║                                                 [Apply] [Cancel]       ║     │║
║└─────║                                                                        ║─────┘║
║┌Foote╚════════════════════════════════════════════════════════════════════════╝─────┐║
║│ [Documentation]        |        KleeneStar v1.2.3        |      [Report a problem] │║
║└────────────────────────────────────────────────────────────────────────────────────┘║
╚══════════════════════════════════════════════════════════════════════════════════════╝
```

It is important to note that the "Show as…" function enables a read-only perspective switch. Write actions such as editing objects, performing workflow transitions, or deleting are fundamentally disabled in this view. The purpose of the function is to check information visibility, not to act on behalf of another person.

When viewing as another person is active, this is clearly indicated at the top edge of the application window. A notice appears with the name of the assumed person and a button to return to one’s own view.

```
╔WebAppPage════════════════════════════════════════════════════════════════════════════╗
║ Viewing as: Max Mustermann [Return to own view]                                    x ║
║┌Header──────────────────────────────────────────────────────────────────────────────┐║
║│ * KleeneStar     Workspace ▼   Dashboard ▼       [+ AddObject]         [Search]    │║
```

### Object Management - Move Function (Modal)

The "Move" function makes it possible to move an existing object to another class or another workspace. This is particularly helpful when the business assignment of a process changes, an object needs to be reclassified, or organizational restructuring takes place.

When moving, all existing values and fields of the object are retained, even if they are no longer valid or intended in the new configuration. Data integrity is deliberately preserved to avoid loss of information and to enable manual post-editing.

After the move, the object is displayed in the new class and/or workspace. Only on the next edit operation is it checked whether the existing values are compatible with the new class schema. Invalid or no longer supported fields must then be adjusted or removed for the object to be saved successfully.

```
╔WebAppPage════════════════════════════════════════════════════════════════════════════╗
║┌Header──────────────────────────────────────────────────────────────────────────────┐║
║│ * KleeneStar     Workspace ▼   Dashboard ▼       [+ AddObject]         [Search]    │║
║└─────╔ObjectMoveModal═════════════════════════════════════════════════════════╗─────┘║
║┌Bread║┌Form──────────────────────────────────────────────────────────────────┐║─────┐║
║│ / Se║│ Move Object                                                          │║     │║
║└─────║├──────────────────────────────────────────────────────────────────────┤║─────┘║
║┌Works║│ You can assign this object to another class or another workspace.    │║─────┐║
║│[Name║│                                                                      │║     │║
║│     ║│ New Workspace: [ Service Desk                                     ▼] │║rch] │║
║│     ║│     New Class: [ Incident                                         ▼] │║     │║
║│     ║│                                                                      │║-----│║
║│ Issu║│                                                                      │║ […] │║
║│ ├─ I║│                                                                      │║ […] │║
║│ ├─ P║│                                                                      │║ […] │║
║│ └─ S║│                                                                      │║ […] │║
║│     ║│                                                                      │║ […] │║
║│     ║│                                                                      │║ […] │║
║│     ║│                                                                      │║ […] │║
║│     ║│                                                                      │║ […] │║
║│     ║│                                                                      │║ […] │║
║│     ║│ Note: All existing values are retained, even if they are not valid   │║ […] │║
║│     ║│       in the new configuration. On the next edit, invalid fields     │║ […] │║
║│     ║│       must be adjusted.                                              │║     │║
║│     ║└──────────────────────────────────────────────────────────────────────┘║xt › │║
║├─────║                                                                        ║     │║
║│ [+] ║                                                       [Move] [Cancel]  ║     │║
║└─────║                                                                        ║─────┘║
║┌Foote╚════════════════════════════════════════════════════════════════════════╝─────┐║
║│ [Documentation]        |        KleeneStar v1.2.3        |      [Report a problem] │║
║└────────────────────────────────────────────────────────────────────────────────────┘║
╚══════════════════════════════════════════════════════════════════════════════════════╝
```

### Object Management - Export (Modal)

The "Export" function enables exporting objects from the system to an external format, for example for further processing, documentation, or sharing with third parties. The function is available in the object overview and may support different formats such as CSV, Excel, or PDF, depending on context.

The export includes all fields and content that are visible to the user according to their permissions. The structure of the export follows the current view. That is, filters, column selection, and sorting directly affect the exported data. Linked objects such as subobjects or parent references can optionally be included, if configured.

The export function is read-only and does not alter any data in the system. It is especially suitable for handing off information to external parties, for archiving completed processes, or for analyzing object data outside the application. For complex objects with nested relationships, the export can be rendered as a structured report, for example with section logic or a hierarchy tree in PDF format.

```
╔WebAppPage════════════════════════════════════════════════════════════════════════════╗
║┌Header──────────────────────────────────────────────────────────────────────────────┐║
║│ * KleeneStar     Workspace ▼   Dashboard ▼       [+ AddObject]         [Search]    │║
║└─────╔ObjectExportModal═══════════════════════════════════════════════════════╗─────┘║
║┌Bread║┌Form──────────────────────────────────────────────────────────────────┐║─────┐║
║│ / Se║│ Export Object                                                        │║     │║
║└─────║├──────────────────────────────────────────────────────────────────────┤║─────┘║
║┌Works║│ Select the desired export format and the content to be exported.     │║─────┐║
║│[Name║│                                                                      │║     │║
║│     ║│ Export format:                                                       │║rch] │║
║│     ║│   ● Excel (.xlsx)                                                    │║     │║
║│     ║│   ○ CSV (.csv)                                                       │║-----│║
║│ Issu║│   ○ PDF report                                                       │║ […] │║
║│ ├─ I║│                                                                      │║ […] │║
║│ ├─ P║│                                                                      │║ […] │║
║│ └─ S║│ Content:                                                             │║ […] │║
║│     ║│   [ ] Linked subobjects                                              │║ […] │║
║│     ║│   [ ] Comments                                                       │║ […] │║
║│     ║│   [ ] History / change log                                           │║ […] │║
║│     ║│                                                                      │║ […] │║
║│     ║│                                                                      │║ […] │║
║│     ║│                                                                      │║ […] │║
║│     ║│    Note: Exported data only includes fields you are allowed to see.  │║ […] │║
║│     ║│                                                                      │║     │║
║│     ║└──────────────────────────────────────────────────────────────────────┘║xt › │║
║├─────║                                                                        ║     │║
║│ [+] ║                                                     [Export] [Cancel]  ║     │║
║└─────║                                                                        ║─────┘║
║┌Foote╚════════════════════════════════════════════════════════════════════════╝─────┐║
║│ [Documentation]        |        KleeneStar v1.2.3        |      [Report a problem] │║
║└────────────────────────────────────────────────────────────────────────────────────┘║
╚══════════════════════════════════════════════════════════════════════════════════════╝
```

### Object Management - Delete Object (Modal)

Deleting an object is a critical action that must be performed with particular care. To ensure this measure is not carried out accidentally, a modal dialog is displayed that prompts the user for explicit confirmation. In this dialog, the object to be deleted is clearly identified, for example by its key such as "INC-00123", and the user must manually enter this key into an input field. Only after correct entry is the delete function enabled. This safety measure protects against unintended data loss, since deletion cannot be undone.

As a rule, deleting objects is only recommended in exceptional cases. A better approach is to keep the object active and instead mark it as "Withdrawn", "Inactive", or "Obsolete" via a status field. This method offers several advantages: traceability is preserved, since historical data remains available, accidental deletions are avoided and there remains the option to reactivate objects or include them in reports if necessary. Using a status field enables transparent, audit-proof, and flexible management of objects without having to carry out irreversible deletion actions.

```
╔WebAppPage════════════════════════════════════════════════════════════════════════════╗
║┌Header──────────────────────────────────────────────────────────────────────────────┐║
║│ * KleeneStar     Workspace ▼   Dashboard ▼       [+ AddObject]         [Search]    │║
║└─────╔ObjectDeleteModal═══════════════════════════════════════════════════════╗─────┘║
║┌Bread║┌Form──────────────────────────────────────────────────────────────────┐║─────┐║
║│ / Se║│ Delete object                                                        │║     │║
║└─────║├──────────────────────────────────────────────────────────────────────┤║─────┘║
║┌Works║│ Are you sure you want to delete object 'INC-00123'?                  │║─────┐║
║│[Name║│ This action cannot be undone.                                        │║     │║
║│     ║│                                                                      │║ […] │║
║│     ║│ To confirm, please type 'INC-00123' into the field below*:           │║rch] │║
║│     ║│ [                                                                  ] │║     │║
║│     ║│                                                                      │║-----│║
║│ Issu║│                                                                      │║ […] │║
║│ ├─ I║│                                                                      │║ […] │║
║│ ├─ P║│                                                                      │║ […] │║
║│ └─ S║│                                                                      │║ […] │║
║│     ║│                                                                      │║ […] │║
║│     ║│                                                                      │║ […] │║
║│     ║│                                                                      │║ […] │║
║│     ║│                                                                      │║ […] │║
║│     ║│                                                                      │║ […] │║
║│     ║│                                                                      │║ […] │║
║│     ║│                                                                      │║ […] │║
║│     ║│                                                                      │║ […] │║
║│     ║│                                                                      │║     │║
║│     ║└──────────────────────────────────────────────────────────────────────┘║xt › │║
║├─────║                                                                        ║     │║
║│ [+] ║                                                     [Delete] [Cancel]  ║     │║
║└─────║                                                                        ║─────┘║
║┌Foote╚════════════════════════════════════════════════════════════════════════╝─────┐║
║│ [Documentation]        |        KleeneStar v1.2.3        |      [Report a problem] │║
║└────────────────────────────────────────────────────────────────────────────────────┘║
╚══════════════════════════════════════════════════════════════════════════════════════╝
```

### Object Management - Permissions Management (Modal)

Access rights to individual objects are managed via a special dialog that can be opened directly from an object’s detail view. This permissions modal serves as a central interface to assign policies to groups and thus control access to objects in a granular way.

To use the modal, users need the `object_manage_profiles` permission. After opening, they can select a group and assign an appropriate access policy (for example, view-only, edit, or full administrative rights). The available policies are divided into three levels:

- `object_view_policy` for read access
- `object_edit_policy` for editing rights
- `object_admin_policy` for full control, including rights management

All existing assignments are displayed in a table. They can be adjusted or extended at any time. To maintain an overview even with extensive permission structures, the modal offers a search function and pagination. Changes to permissions take effect immediately and are logged for traceability.

```
╔WebAppPage════════════════════════════════════════════════════════════════════════════╗
║┌Header──────────────────────────────────────────────────────────────────────────────┐║
║│ * KleeneStar     Workspace ▼   Dashboard ▼       [+ AddObject]         [Search]    │║
║└─────╔WorkspacePermissionsModal═══════════════════════════════════════════════╗─────┘║
║┌Bread║┌Form──────────────────────────────────────────────────────────────────┐║─────┐║
║│ / Se║│  Manage Permissions for 'INC-00123'                                  │║     │║
║└─────║├──────────────────────────────────────────────────────────────────────┤║─────┘║
║┌Works║│                                                                      │║─────┐║
║│[Name║│  Assign Group*: [ Admin ▼]                                           │║     │║
║│     ║│        Policy*: [ object_admin_policy ▼]                             │║ […] │║
║│     ║│                                                                      │║rch] │║
║│     ║│  [+ Assign]                                                          │║     │║
║│     ║│                                                             [Search] │║---- │║
║│ Issu║│                                                                      │║ […] │║
║│ ├─ I║│ Assigned Group       | Effective Policy                            + │║ […] │║
║│ ├─ P║│----------------------|-----------------------------------------------│║ […] │║
║│ └─ S║│ Any                  | object_view_policy                          X │║ […] │║
║│     ║│ IT                   | object_edit_policy                          X │║ […] │║
║│     ║│                                                                      │║ […] │║
║│     ║│                                             ‹ Prev  1  2  3  Next ›  │║ […] │║
║│     ║│                                                                      │║ […] │║
║│     ║│                                                                      │║ […] │║
║│     ║│                                                                      │║ […] │║
║│     ║│                                                                      │║ […] │║
║│     ║│                                                                      │║ […] │║
║│     ║│                                                                      │║     │║
║│     ║└──────────────────────────────────────────────────────────────────────┘║xt › │║
║├─────║                                                                        ║     │║
║│ [+] ║                                                                [Done]  ║     │║
║└─────║                                                                        ║─────┘║
║┌Foote╚════════════════════════════════════════════════════════════════════════╝─────┐║
║│ [Documentation]        |        KleeneStar v1.2.3        |      [Report a problem] │║
║└────────────────────────────────────────────────────────────────────────────────────┘║
╚══════════════════════════════════════════════════════════════════════════════════════╝
```

## Sitemap

The sitemap describes the structured navigation paths for accessing objects within a workspace and forms the basis for a consistent and intuitive user experience in the application. All routes are organized hierarchically and follow the logical structure by workspaces and object classes.

Each route fulfills a specific function, from the overview of all objects of a class to detail view, editing, or deletion of individual entries. This clear structure ensures that users can always navigate between different views and actions in a traceable and efficient manner.

The sitemap supports both technical integration and conceptual design of the user interface and significantly contributes to the system’s usability and maintainability.

|Path                                                       |Page/View               |Description
|-----------------------------------------------------------|------------------------|--------------------------------------------------------------
|`/search`                                                  |Object search           |Global, workspace-wide search for objects.
|`/workspaces/{workspaceKey}`                               |Object overview         |Lists all objects of the specified class in the workspace.
|`/workspaces/{workspaceKey}/add`                           |Object creation         |Opens the modal for selecting an object template to create a new object.
|`/workspaces/{workspaceKey}/{objectKey}`                   |Object detail view      |Detailed view of a single object.
|`/workspaces/{workspaceKey}/{objectKey}/edit`              |Object editing          |Form for editing the metadata of an existing object.
|`/workspaces/{workspaceKey}/{objectKey}/delete`            |Object deletion         |Confirmation and execution of the deletion process.
|`/workspaces/{workspaceKey}/{objectKey}/move`              |Object move             |Move an object to another workspace or class.
|`/workspaces/{workspaceKey}/{objectKey}/export`            |Object export           |Export an object in different formats.
|`/workspaces/{workspaceKey}/{objectKey}/permissions`       |Permissions management  |Manage access rights for an object.
|`/workspaces/{workspaceKey}/{objectKey}/link`              |Linking                 |Add links to an object.
|`/workspaces/{workspaceKey}/{objectKey}/subobject`         |Subobject creation      |Start the process to create a subobject.
|`/workspaces/{workspaceKey}/{objectKey}/show-as`           |View-as                 |View the object from another user’s perspective.
|`/workspaces/{workspaceKey}/{objectKey}/transition/{tKey}` |Workflow transition     |Execute a workflow transition and, if applicable, display a form.

## API Interfaces (REST Endpoints)

For programmatic interaction with object management, **KleeneStar** provides a REST-compliant API. This interface uses JSON as the data format and is fully integrated into **KleeneStar**’s authentication and authorization model. The API enables access to objects within workspaces and supports all relevant CRUD operations.

|Endpoint                                                 |HTTP Method |Description
|---------------------------------------------------------|------------|-----------
|`/api/1/objects/search`                                  |POST        |Performs a cross-workspace search for objects using WQL.
|`/api/1/workspaces/{workspaceKey}/objects`               |GET         |Lists all objects in the workspace. Supports filtering (by class, fields) and pagination.
|`/api/1/workspaces/{workspaceKey}/objects`               |POST        |Creates a new object. Requires `classKey` and initial field values in the request body.
|`/api/1/objects/{objectKey}`                             |GET         |Retrieves detailed information for a specific object.
|`/api/1/objects/{objectKey}`                             |PUT         |Updates the field values of an existing object.
|`/api/1/objects/{objectKey}`                             |DELETE      |Deletes an object. Requires appropriate permissions.
|`/api/1/objects/{objectKey}/archive`                     |POST        |Archives an object and puts it into a write-protected state.
|`/api/1/objects/{objectKey}/restore`                     |POST        |Restores a previously archived object.
|`/api/1/objects/{objectKey}/transitions`                 |GET         |Lists all available workflow transitions for the object.
|`/api/1/objects/{objectKey}/transitions/{transitionKey}` |POST        |Executes a workflow transition for the object. May include field values in the body.

Standard error responses include `400 Bad Request` for validation errors (e.g., a key that is already taken), `401 Unauthorized` for missing authentication, `403 Forbidden` for insufficient permissions, and `404 Not Found` if the requested resource does not exist. A successful creation (POST) is acknowledged with `201 Created`, while a successful deletion (DELETE) results in a `204 No Content` response.

## Events

Object management is based on an event-driven architecture that communicates state changes system-wide transparently, traceably, and in real time. Every relevant action on an object, such as creation, update, deletion, or a workflow transition, triggers a specific event that is published via the central **WebExpress** `EventManager`.

These events can be subscribed to and processed by other system components, integrations, or external services. Typical use cases include logging changes, triggering automated follow-up processes, synchronization with third-party systems, or sending notifications to users or systems.

The architecture enables loose coupling between components, making the system flexibly extensible.

|Event name          |Description
|--------------------|--------------------------------------------------------------------
|`ObjectCreated`     |Triggered when a new object has been created successfully.
|`ObjectUpdated`     |Signals changes to field values of an existing object.
|`ObjectDeleted`     |Indicates the permanent deletion of an object.
|`ObjectArchived`    |Marks an object as archived and write-protected.
|`ObjectRestored`    |Reports the restoration of a previously archived object.
|`ObjectMoved`       |Triggered when an object is moved to another workspace or class.
|`ObjectTransitioned`|Triggered when an object successfully undergoes a workflow transition.
|`ObjectLinked`      |Signals that an object was linked to another object or an external resource.
|`ObjectUnlinked`    |Signals that a link was removed from an object.
|`ObjectCloned`      |Triggered when an object has been cloned, and includes references to source and target objects.

## Permissions Model

The permissions model for object management in **KleeneStar** is based on a context-sensitive and derivative approach. Access rights to individual objects are not granted directly at the object level. Instead, they derive from permissions in the higher-level contexts, specifically the workspace, the object class, and the associated fields. This consolidated rights assignment ensures consistent, maintainable, and traceable access control throughout the system.

In principle, a user only gains access to an object if they have the corresponding rights across all relevant levels. Read rights require that the user has read access to the workspace and the class, and can read at least one visible field of the object. Write rights additionally require permission to edit the affected fields. Creating new objects is only possible if the `class_create_objects` permission is present for the respective class. Similarly, object deletion is tied to the `class_delete_objects` permission. For workflow transitions, the `class_transition_execute` permission must be present, either globally for the workflow or specifically for individual transitions.

In addition to these derived rights, object-specific restrictions can be defined at the object level, but exclusively for read and write access. These object-related restrictions are restrictive and allow fine-grained control, such as blocking access for certain groups or roles. They can restrict the rights derived from the contexts but never extend them.

Object-related permissions control which actions a user may perform on individual objects within a workspace. They are derived from the higher-level contexts (workspace, class, field) but can be further reduced by object-specific restrictions on read and write access.

|Permission            |Description
|----------------------|-------------------------------------------------------------------
|`object_read`         |Grants read access to an object’s fields and metadata.
|`object_update`       |Authorizes editing of existing objects (depending on field rights).
|`object_comment`      |Allows adding and editing comments on an object.
|`object_attach`       |Enables uploading and managing attachments for an object.
|`object_linking`      |Allows managing object links (e.g., to issues or external resources).
|`object_manage_profiles`|Allows management of object policies and their assignment to groups.

Policies bundle individual permissions into meaningful role profiles and enable context-based assignment via workspace profiles. They define which actions users are allowed to perform on objects, depending on their group membership and the profile assigned in the respective workspace.

|Policy                 |Description                                                       |Included permissions
|-----------------------|------------------------------------------------------------------|-------------------------------
|`object_admin_policy`  |Full administrative control over all objects in the workspace.    |All `object_*` permissions
|`object_edit_policy`   |Entitled to actively edit and comment on objects.                 |`object_read`, `object_update`, `object_comment`, `object_attach`, `object_linking`
|`object_view_policy`   |Grants read-only access to objects and their content.             |`object_read`

## Conclusion

The object management concept defines the fundamental handling of data instances in **KleeneStar**. It integrates seamlessly with the existing concepts of workspaces, classes, fields, workflows, and forms, creating a coherent system as a whole. The `ObjectManager` serves as the central instance that governs CRUD operations, lifecycle management, and enforcement of business rules and permissions. The UI concepts and the REST API provide flexible and intuitive interfaces for users and external systems. For a complete implementation, technical details such as concurrent access, performance optimization for large data volumes, and caching strategies must be elaborated further.
