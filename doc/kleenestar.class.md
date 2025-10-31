# KleeneStar Class Management Concept

In **KleeneStar**, the term "class" denotes a structured template for describing data, comparable to a blueprint that defines which fields, rules, and relationships an object can have. These classes form the basis of a workspace’s data model. Unlike programming classes in object-oriented programming, they do not contain methods or logic; they serve exclusively for the semantic structuring and validation of data. Through centrally managed base workspaces, classes can be defined uniformly across the system and automatically inherited. This results in a consistent, versionable, and traceable data model that combines central governance with local extensibility. Classes consolidate all relevant information on the data structure (from field definitions and validation rules to search indexes), thereby ensuring data quality, performance, and interoperability. Local adaptations are possible but clearly delineated, so central governance is preserved and changes remain traceable at all times.

Classes bundle:
- Field definitions including types, cardinalities, default values, indexes, and validation.
- Linking rules to other classes with cardinality and consistency constraints.
- Validation and business rules for cross-field consistency.
- Schema versions including declarative migration paths for compatible and incompatible changes.

## Lifecycle and States

Class management in **KleeneStar** follows a default lifecycle that includes the states active, archived, and deleted. This model simplifies administrative processes while ensuring stability and traceability. When created, a class is immediately initialized as active and available for productive use. In this state, to maintain data integrity, only compatible schema changes or versioned migrations are permitted.

Classes no longer used actively can be moved to the archived state. As a read-only archive, this state serves pure historization of data. A resumption of use is enabled via the restore operation, which returns the class to the active state. The final removal of a class is initiated by transitioning to the deleted state. This irreversible step is possible from both the active and archived states. The actual deletion occurs afterward, taking into account defined retention periods and dependency checks.

The following state diagram visualizes this default lifecycle:

```
╔══════════════════════════════════════════════════════════════════════════════════════╗
║                       KleeneStar Class Lifecycle State Diagram                       ║
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

**KleeneStar**’s core data model is based on the defined class structures and forms the foundation for managing identities, roles, and permissions. It follows a modular architecture and clearly separates type definitions (Class), concrete instances (Object), and semantic extensions such as comments or versions.

The central entity is the Class, which acts as a blueprint for data objects and defines their fields (Field). A Workspace functions as a container that bundles multiple such classes and thus defines the scope of a data model.

Concrete data instances are represented as Object and are always assigned to a Class. The actual values are stored in Value instances, which establish a connection between an Object and a Field. Relationships between objects are modeled via Link instances. In addition, metadata such as Comment, Version, and FileReference can be attached to an Object to improve traceability and documentation.

```
╔══════════════════════════════════════════════════════════════════════════════════════╗
║                             KleeneStar Core Data Model                               ║
╠══════════════════════════════════════════════════════════════════════════════════════╣
║                                                                                      ║
║         ┌───────────┐ *           * ┌───────┐ 1          * ┌───────┐                 ║
║         │ Workspace ├──────────────►│ Class │◄─────────────┤ Field │                 ║
║         └─────┬─────┘               └───────┘              └───────┘                 ║
║               │ 1                       ▲ 1                    ▲ 1                   ║
║               └────────────────────┐    │                      │                     ║
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

The application follows a modular, decoupled architectural principle. At its center is the `ClassManager`, which is exclusively responsible for the lifecycle and access to all classes. It manages a collection of Class instances and provides a controlled interface for all interactions.

Each instance contains key attributes such as name, workspace affiliation, and lifecycle status. New classes are created exclusively via the `ClassManager` to ensure data integrity and consistent access rules.

For a loosely coupled, reactive architecture, the `ClassManager` provides the events addclass and removeclass. Other components can subscribe to these and react to changes without being directly dependent on the manager. This event system fosters modularity and high cohesion.

The `ClassManager` handles server-side tasks such as the persistent storage of all classes in a transactional, versioned store. At system startup, stored classes are loaded, indexes are built, and event subscriptions are initialized. To support fast and context-aware searches, a server-side reverse index is created for each class. This captures keywords from names, descriptions, user-defined tags, as well as structured metadata such as creation date and status. The index is continuously updated and enables high-performance full-text and metadata searches.

On every request, the `ClassManager` enforces authorization for the calling module or user. Access is governed by policies that may include context-dependent filters, time-limited entitlements, or audit constraints, enabling a flexible and fine-grained implementation of differentiated read and write permissions.

An integrated audit system documents all relevant actions around classes: accesses, changes, context switches, and permission checks are logged with timestamp, user identity, class key, and action type. This data supports analysis, troubleshooting, compliance verification, and state restoration.

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
║         ┌-----------┤ IClassManager                         │                        ║
║         ¦           ├───────────────────────────────────────┤                        ║
║         ¦           │ AddClass:Event                        │                        ║
║         ¦           │ UpdateClass:Event                     │                        ║
║         ¦           │ RemoveClass:Event                     │                        ║
║         ¦           ├───────────────────────────────────────┤ 1                      ║
║         ¦           │ Classes:IEnumerable<IClass>           ├───────┐                ║
║         ¦           ├───────────────────────────────────────┤       │                ║
║         ¦           │ AddClass(IWorkspace,IClass):IClass    │       │                ║
║         ¦           │ GetClasses(IWorkspace,filter):        │       │                ║
║         ¦           │   IEnumerable<IClass>                 │       │                ║
║         ¦           │ CloneClass(IWorkspace,IClass):IClass  │       │                ║
║         ¦           │ DeleteClass(IWorkspace,IClass):bool   │       │                ║
║         ¦           └───────────────────────────────────────┘       │                ║
║         ¦                                                           │                ║
║         ¦                      ┌───────────────┐                    │                ║
║         ¦                      │ <<Interface>> │                    │                ║
║         ¦                      │ IModel        │                    │                ║
║         ¦                      ├───────────────┤                    │                ║
║         ¦                      └───────────────┘                    │                ║
║         ¦                             Δ                             │                ║
║         ¦                             ¦                             │                ║
║         ¦                             ¦                             │                ║
║         ¦           ┌─────────────────┴──────────────────┐ *        │                ║
║         ¦           │ <<Interface>>                      │◄─────────┘                ║
║         ¦           │ IClass                             │        ┌────────────────┐ ║
║         ¦           ├────────────────────────────────────┤        │ <<Enum>>       │ ║
║         ¦           │ Key:String                         │        │ TypeClassState │ ║
║         ¦           │ Name:String                        │        ├────────────────┤ ║
║         ¦           │ State:TypeClassState               │        │ Active         │ ║
║         ¦           │ Workspace:IWorkspace               │        │ Archived       │ ║
║         ¦           │ Created:DateTime                   │        └────────────────┘ ║
║         ¦           │ Updated:DateTime                   │    ┌────────────────────┐ ║
║         ¦           │ IsSubClass:Bool                    │    │ <<Enum>>           │ ║
║         ¦           │ IsAbstract:Bool                    │    │ TypeAccessModifier │ ║
║         ¦           │ Inherited:IClass                   │    ├────────────────────┤ ║
║         ¦           │ Sealed:Bool                        │    │ Private            │ ║
║         ¦           │ Parent:IClass                      │    │ Protected          │ ║
║         ¦           │ AccessModifier:TypeAccessModifier  │    │ Public             │ ║
║         ¦           │ Fields:IEnumerable<IField>         │    │ Internal           │ ║
║         ¦           │ Subclasses:                        │    └────────────────────┘ ║
║         ¦           │   IEnumerable<IClass>              │                           ║
║         ¦           │ PermissionsProfiles:               │                           ║
║         ¦           │   IEnumerable<IPermissionsProfile> │                           ║
║         ¦           └────────────────────────────────────┘                           ║
║         ¦                             Δ                                              ║
║         ¦                             ¦                                              ║
║         ¦                             ¦                                              ║
║         ¦ create    ┌─────────────────┴──────────────────┐                           ║
║         └----------►│ Class                              │                           ║
║                     ├────────────────────────────────────┤                           ║
║                     │ Key:String                         │                           ║
║                     │ Name:String                        │                           ║
║                     │ State:TypeClassState               │                           ║
║                     │ Workspace:Workspace                │                           ║
║                     │ Created:DateTime                   │                           ║
║                     │ Updated:DateTime                   │                           ║
║                     │ IsSubClass:Bool                    │                           ║
║                     │ IsAbstract:Bool                    │                           ║
║                     │ Inherited:IClass                   │                           ║
║                     │ Sealed:Bool                        │                           ║
║                     │ Parent:IClass                      │                           ║
║                     │ AccessModifier:TypeAccessModifier  │                           ║
║                     │ Fields:IEnumerable<Field>          │                           ║
║                     │ Subclasses:                        │                           ║
║                     │   IEnumerable<IClass>              │                           ║
║                     │ PermissionsProfiles:               │                           ║
║                     │   IEnumerable<IPermissionsProfile> │                           ║
║                     └────────────────────────────────────┘                           ║
║                                                                                      ║
╚══════════════════════════════════════════════════════════════════════════════════════╝
```

The class model in **KleeneStar** enables the definition and maintenance of a wide range of class types, supporting both simple structures and complex recursive models. Classes can represent standalone object types with structured attributes such as title, status, priority, or description. More advanced configurations can be modeled directly through inheritance, composition, and hierarchical classification.

Each class can inherit exactly one other class. Inheritance is referential, meaning that fields and behaviors from the parent class become visible in the child without being copied. Resolution follows a clear precedence: overrides in the child take priority over definitions in the parent. If no override is present, the parent’s values apply. Child-specific overloads can selectively hide inherited elements without modifying the parent. In case of naming conflicts, the system deterministically favors the child. The operational state remains isolated per class. Archiving a parent class does not automatically affect its children but is displayed as a source status.

Class visibility is controlled via the Access Modifier, which follows a monotonic rule across the inheritance hierarchy. A child class may only adopt the same or a more restrictive visibility than its parent and never a broader one. The available levels include Private, which ensures complete encapsulation and disallows visibility outside the class itself. Protected allows visibility for derived classes. Internal, also referred to as Tenant, enables visibility within the same tenant. Public allows cross-tenant visibility and is required for cross-tenant inheritance. Sealed marks a class as inheritable but prevents further extension beyond its immediate children. A sealed class can be inherited but cannot itself be extended further.

**KleeneStar** also supports composition through the Subclasses field. This allows embedding other class types to model sub-issues, comments, approvals, or tasks. Classes can be marked as abstract, meaning they cannot be instantiated directly but serve as reusable base types. Through the Parent field, a class can be assigned to a higher-level category such as Issue or Request, enabling hierarchical classification.

Further options include the Singleton flag, which ensures that only one instance of the class may exist per context. This is useful for modeling settings, profiles, or configuration anchors. Attributes can be added dynamically via Add Field, with support for types such as String, Enum, DateTime, or Text.

## UI Concepts and Pages

The following UI mockups show how the complex structures and rules of class management in **KleeneStar** are translated into a comprehensible and user-friendly interface. The goal is to make working with classes as intuitive, efficient, and safe as possible.

The user interface consistently follows the established design patterns of the **KleeneStar** web application. This creates a familiar user experience with clear flows and recognizable elements. Onboarding remains brief, and typical tasks can be completed quickly and reliably.

The mockups serve as a visual template for the final UI design. They show how navigation is structured, where important controls are located, and how different system states (such as active, archived, or deleted classes) are presented. Concrete workflows illustrate how to create, edit, archive, or remove classes. Particular emphasis is placed on clarity, feedback during actions, and adherence to permissions.

### Class Management in Workspace Editing (Sidebar)

In the workspace editing view, the "Manage Classes" function is accessible via the settings dropdown in the sidebar. It points to a separate management page where all classes assigned to the workspace can be centrally maintained. The page provides a tabular overview with information such as name, description, and status. New classes can be created there, and existing ones can be edited, cloned, or deleted. In addition, composition rules and permission profiles can be adjusted.

Additionally, the sidebar contains an "Add Class" feature that allows new classes to be created directly in the current workspace context. This integration enables structured maintenance of class definitions without leaving the editing view.

```
╔WebAppPage════════════════════════════════════════════════════════════════════════════╗
║┌Header──────────────────────────────────────────────────────────────────────────────┐║
║│ * KleeneStar     Workspace ▼                     [+ AddObject]                     │║
║└────────────────────────────────────────────────────────────────────────────────────┘║
║┌Breadcrumb──────────────────────────────────────────────────────────────────────────┐║
║│ / Workspace 0                                                                      │║
║└────────────────────────────────────────────────────────────────────────────────────┘║
║┌Workspace─────────────┐ ┌Workspace Content──────────────────────────────────────────┐║
║│[Name]                │░│                                                           │║
║│                      │░│ Incident                                              […] │║
║│      [Icon]          │░│                                                  [Search] │║
║│                      │░│ Title                            | Status  | Impact       │║
║│             [Search] │░│----------------------------------|---------|------------- │║
║│ Issue                │░│ VPN connection disrupted         | Open    | High     […] │║
║│ ├─ Incident          │░│ Outlook won't start              | Open    | Medium   […] │║
║│ ├─ Problem           │░│ Printer on floor 3 offline       | Assigned| Low      […] │║
║│ └─ ServiceRequest    │░│ File upload fails                | In Prog.| Medium   […] │║
║│                      │░│ Remote desktop not reachable     | Open    | High     […] │║
║│    ┌Workspace───────┐│<│ Password reset not possible      | Closed  | Low      […] │║
║│    │ Edit           ││<│ Wi-Fi outage in conference room  | Open    | High     […] │║
║│    │ Clone          ││<│ Teams notifications delayed      | Assigned| Medium   […] │║
║│    │ Manage Classes ││░│ Scanner not sending PDFs         | Assigned| Low      […] │║
║│    │ Permissions    ││░│ SharePoint access denied         | Open    | High     […] │║
║│    │ <section>      ││░│ Software update blocks startup   | In Prog.| High     […] │║
║│    ├────────────────┤│░│ Screen flickers intermittently   | Closed  | Medium   […] │║
║│    │ Delete         ││░│                                                           │║
║│    └───┬────────────┘│░│                                   ‹ Prev  1  2  3  Next › │║
║├────────¦─────────────┤░│                                                           │║
║│ [+] | [Setting]   << │░│                                                           │║
║└──────────────────────┘ └───────────────────────────────────────────────────────────┘║
║┌Footer──────────────────────────────────────────────────────────────────────────────┐║
║│                                                                                    │║
║└────────────────────────────────────────────────────────────────────────────────────┘║
╚══════════════════════════════════════════════════════════════════════════════════════╝
```

### Class Management (Page)

This page serves as the central management hub for all classes defined in the system. It provides a comprehensive overview of the available class types and offers functions for creation, editing, and organization.

The main area contains a tabular list of all classes. Each row shows key attributes such as name, description, and status. For better orientation with large inventories, search and filter functions are integrated directly into the interface. The page supports functions such as editing, cloning, deleting, and archiving classes, as well as managing permissions. New class definitions can be created via the "Add Class" button.

The page is accessible via the "Manage Classes" option in the workspace editing settings dropdown. It forms the basis for the structured maintenance of class types within a workspace context. Changes to classes have an immediate impact on the available object types and their behavior in the respective workspace.

```
╔WebAppPage════════════════════════════════════════════════════════════════════════════╗
║┌Header──────────────────────────────────────────────────────────────────────────────┐║
║│ * KleeneStar     Workspace ▼                     [+ AddObject]                     │║
║└────────────────────────────────────────────────────────────────────────────────────┘║
║┌Breadcrumb──────────────────────────────────────────────────────────────────────────┐║
║│ / Workspace 0 / Classes                                                            │║
║└────────────────────────────────────────────────────────────────────────────────────┘║
║┌Classes───────────────┐ ┌Classes Content────────────────────────────────────────────┐║
║│                      │░│                                                           │║
║│  - All               │░│                                    [Search] [+ Add Class] │║
║│  - Issues            │░│                                                           │║
║│  - Sub-Issues        │░│ Class Name       | Description                 │ Status   │║
║│  - Hidden            │░│------------------|-----------------------------│----------│║
║│  - Archived          │░│ Incident         | Report of a disruption      │ ...  […] │║
║│                      │░│ Problem          | Analysis of recurring errors│ ...   ¦  │║
║│                      │░│ ChangeRequest    | Request for change   ┌──────────────┴┐ │║
║│                      │░│ ServiceRequest   | Standard service requ│ Edit          │ │║
║│                      │░│ KnowledgeArticle | Documented knowledge │ Clone         │ │║
║│                      │<│ Approval         | Approval step        │ Manage Fields │ │║
║│                      │<│ Request          | Inquiry or sub-proces│ Permissions   │ │║
║│                      │<│ Task             | Executable activity  │ <section>     │ │║
║│                      │░│ SLA              | Service Level Agreeme├───────────────┤ │║
║│                      │░│ Comment          | Free-text note       │ Delete        │ │║
║│                      │░│ UserFeedback     | User feedback        └───────────────┘ │║
║│                      │░│ Escalation       | Escalation to higher inst...│ ...  […] │║
║│                      │░│                                                           │║
║│                      │░│                                   ‹ Prev  1  2  3  Next › │║
║├──────────────────────┤░│                                                           │║
║│ [Setting]         << │░│                                                           │║
║└──────────────────────┘ └───────────────────────────────────────────────────────────┘║
║┌Footer──────────────────────────────────────────────────────────────────────────────┐║
║│                                                                                    │║
║└────────────────────────────────────────────────────────────────────────────────────┘║
╚══════════════════════════════════════════════════════════════════════════════════════╝
```

### Class Management - New/Edit (Modal)

The "Add Class" and "Edit Class" pages in the **KleeneStar** web application offer a central interface for creating and editing classes. They translate the abstract structure of the data model into a clearly structured form used both for new entries and for maintaining existing classes. 

When adding a class ("Add Class"), users start by entering basic information such as name, description, and, if desired, a parent class from which properties can be inherited. It can also be specified whether the class should be abstract, i.e., not directly instantiable. A central element is the selection of a class type that describes the functional role of the class in the system (e.g., "Document", "Enumeration", "Issue", "Log"). This type influences the presentation, available fields, and system-side processing. It is not a template in the classic sense but a semantic classification that governs the behavior and integration of the class within the overall system. In addition to manual entry, **KleeneStar** supports the use of blueprints that automatically provide suitable metadata and field definitions based on the selected class type. These blueprints contain structured suggestions for typical attributes, data types, and descriptions appropriate for the respective type. For example, a class of type "Issue" can automatically include fields such as "Title", "Status", "Priority", and "Assignee". The suggested fields can be customized, extended, or removed after insertion. This creates a flexible starting point that accelerates modeling while promoting consistent structures—particularly useful in larger projects or with recurring class types.

When editing a class ("Edit Class"), all existing information is pre-populated. Users can adjust name, description, type, and fields. Changes are saved via the "Save" button, while "Cancel" aborts the process.

```
╔WebAppPage════════════════════════════════════════════════════════════════════════════╗
║┌Header──────────────────────────────────────────────────────────────────────────────┐║
║│ * KleeneStar     Workspace ▼                     [+ AddObject]                     │║
║└─────╔ClassAddEditModal═══════════════════════════════════════════════════════╗─────┘║
║┌Bread║┌Form──────────────────────────────────────────────────────────────────┐║─────┐║
║│ / Si║│ Add Class / Edit Class                                               │║     │║
║└─────║├──────────────────────────────────────────────────────────────────────┤║─────┘║
║┌Class║│           Name: [Incident                                          ] │║─────┐║
║│     ║│    Description: [Report of technical or operational incidents.     ] │║     │║
║│  - A║│      Inherited: [None                                             ▼] │║ass] │║
║│  - I║│       Abstract: [ ]                                                  │║     │║
║│  - S║│      Blueprint: [Incident                                         ▼] │║     │║
║│  - H║│         Parent: [Issue                                            ▼] │║---- │║
║│  - A║│     Subclasses: [                                                 ▼] │║ […] │║
║│     ║│Access Modifier: [Private                                          ▼] │║ […] │║
║│     ║│      Singelton: [ ]                                                  │║ […] │║
║│     ║│         Active: [✓]                                                  │║ […] │║
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
║│ [+ A║                                                       [Save] [Cancel]  ║     │║
║└─────║                                                                        ║─────┘║
║┌Foote╚════════════════════════════════════════════════════════════════════════╝─────┐║
║│                                                                                    │║
║└────────────────────────────────────────────────────────────────────────────────────┘║
╚══════════════════════════════════════════════════════════════════════════════════════╝
```

### Class Management – Clone (Modal)

Cloning a class enables fast reuse of existing structures and is particularly suitable for leveraging proven class definitions. The function is provided via a separate modal dialog that can be opened from both the detail view and the class overview. 

When cloning, a new class is created whose properties (such as name, description, fields, and metadata) are copied from the original. System-critical characteristics such as the unique key, creation timestamp, and permissions are regenerated or specifically requested. The modal allows for adjusting the new class’s name and description. Optionally, it can be specified whether to include the field structure and associated permissions.

After confirmation, the new class is created and automatically integrated into the workspace’s existing class list.

```
╔WebAppPage════════════════════════════════════════════════════════════════════════════╗
║┌Header──────────────────────────────────────────────────────────────────────────────┐║
║│ * KleeneStar     Workspace ▼                     [+ AddObject]                     │║
║└─────╔ClassCloneModal═════════════════════════════════════════════════════════╗─────┘║
║┌Bread║┌Form──────────────────────────────────────────────────────────────────┐║─────┐║
║│ / Wo║│ Clone Class                                                          │║     │║
║└─────║├──────────────────────────────────────────────────────────────────────┤║─────┘║
║┌Class║│                                                                      │║─────┐║
║│     ║│ You are about to clone the class 'Sales Operations'.                 │║     │║
║│  - A║│ Please adjust the details for the new workspace below.               │║ass] │║
║│  - I║│                                                                      │║     │║
║│  - S║│     Class Name: [ xxxxxx (Copy)                                  ]   │║     │║
║│  - H║│            Key: [ newkey                                         ]   │║---- │║
║│  - A║│    Description: [ Copy of xxxxxx.                                ]   │║ […] │║
║│     ║│                                                                      │║ […] │║
║│     ║│   Include structure: [✓]                                             │║ […] │║
║│     ║│ Include permissions: [✓]                                             │║ […] │║
║│     ║│                                                                      │║ […] │║
║│     ║│                                                                      │║ […] │║
║│     ║│                                                                      │║ […] │║
║│     ║│                                                                      │║ […] │║
║│     ║│                                                                      │║     │║
║│     ║└──────────────────────────────────────────────────────────────────────┘║xt › │║
║├─────║                                                                        ║     │║
║│ [+ A║                                                      [Clone] [Cancel]  ║     │║
║└─────║                                                                        ║─────┘║
║┌Foote╚════════════════════════════════════════════════════════════════════════╝─────┐║
║│                                                                                    │║
║└────────────────────────────────────────────────────────────────────────────────────┘║
╚══════════════════════════════════════════════════════════════════════════════════════╝
```

### Class Management – Delete (Modal)

Deleting a class is a critical and irreversible process that requires explicit user confirmation. To prevent accidental deletions, a modal dialog is used. This modal is activated when the delete action is initiated from either the detail view or the class overview.

The dialog clearly indicates which class is to be deleted by explicitly naming the class and its key. As an additional safety measure, the user must type the class key into an input field. Only when the input matches the actual key does the "Delete" button become enabled. This ensures that deletion is deliberate and intentional. The modal also provides a "Cancel" option to abort the process without making changes.

```
╔WebAppPage════════════════════════════════════════════════════════════════════════════╗
║┌Header──────────────────────────────────────────────────────────────────────────────┐║
║│ * KleeneStar     Workspace ▼                     [+ AddObject]                     │║
║└─────╔═ClassDeleteModal═══════════════════════════════════════════════════════╗─────┘║
║┌Bread║┌Form──────────────────────────────────────────────────────────────────┐║─────┐║
║│ / Wo║│ Delete Class                                                         │║     │║
║└─────║├──────────────────────────────────────────────────────────────────────┤║─────┘║
║┌Class║│                                                                      │║─────┐║
║│     ║│ Are you sure you want to delete the class 'Incident'?                │║     │║
║│  - A║│ This action cannot be undone.                                        │║ass] │║
║│  - I║│                                                                      │║     │║
║│  - S║│ To confirm, please type 'incident-001' in the box below:             │║     │║
║│  - H║│ [                                                                  ] │║---- │║
║│  - A║│                                                                      │║ […] │║
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
║│ [+ A║                                                     [Delete] [Cancel]  ║     │║
║└─────║                                                                        ║─────┘║
║┌Foote╚════════════════════════════════════════════════════════════════════════╝─────┐║
║│                                                                                    │║
║└────────────────────────────────────────────────────────────────────────────────────┘║
╚══════════════════════════════════════════════════════════════════════════════════════╝
```

### Class Management – Permissions Management (Modal)

A modal dialog is used to manage access rights to a class, serving as the central interface for assigning groups to context-specific permission policies. This modal enables fine-grained control over which groups can access a specific class with which rights. Access to the modal is via the "Permissions" button in class management. The permission `class_manage_profiles` is required to display and use the dialog. Within the modal, administrators can select groups and assign suitable policies to them.

Assignments are displayed in a tabular overview and can be adjusted or removed at any time.

```
╔WebAppPage════════════════════════════════════════════════════════════════════════════╗
║┌Header──────────────────────────────────────────────────────────────────────────────┐║
║│ * KleeneStar     Workspace ▼                     [+ AddObject]                     │║
║└─────╔ClassPermissionsModal═══════════════════════════════════════════════════╗─────┘║
║┌Bread║┌Form──────────────────────────────────────────────────────────────────┐║─────┐║
║│ / Cl║│  Manage Permissions for Class 'Incident'                             │║     │║
║└─────║├──────────────────────────────────────────────────────────────────────┤║─────┘║
║┌Class║│                                                                      │║─────┐║
║│     ║│  Assign Group: [IT Support ▼]                                        │║     │║
║│  - A║│        Policy: [class_edit_policy ▼]                                 │║ass] │║
║│  - I║│                                                                      │║     │║
║│  - S║│  [+ Assign]                                                          │║     │║
║│  - H║│                                                             [Search] │║---- │║
║│  - A║│                                                                      │║ […] │║
║│     ║│ Assigned Group       | Effective Policy                              │║ […] │║
║│     ║│----------------------|-----------------------------------------------│║ […] │║
║│     ║│ IT Support           | class_edit_policy                           X │║ […] │║
║│     ║│ Service Desk         | class_view_policy                           X │║ […] │║
║│     ║│ Incident Managers    | class_admin_policy                          X │║ […] │║
║│     ║│                                                                      │║ […] │║
║│     ║│                                             ‹ Prev  1  2  3  Next ›  │║ […] │║
║│     ║│                                                                      │║     │║
║│     ║└──────────────────────────────────────────────────────────────────────┘║xt › │║
║├─────║                                                                        ║     │║
║│ [+][║                                                                [Done]  ║     │║
║└─────║                                                                        ║─────┘║
║┌Foote╚════════════════════════════════════════════════════════════════════════╝─────┐║
║│                                                                                    │║
║└────────────────────────────────────────────────────────────────────────────────────┘║
╚══════════════════════════════════════════════════════════════════════════════════════╝
```

## Sitemap

The sitemap defines navigation paths and visibility logic within the application. It forms the basis for routing and ensures a consistent structure relative to workspace routes. Clear path definitions enable deep-linkable addressing of individual class states, for example for detail views, editing dialogs, or permission management.

|Path                                                       |Page              |Description
|-----------------------------------------------------------|------------------|------------------------------------------------------------
|`/workspaces/{workspaceKey}/classes`                       |Class Management  |Overview of all classes defined in the workspace.
|`/workspaces/{workspaceKey}/classes/add`                   |Class Creation    |Form for creating a new local class.
|`/workspaces/{workspaceKey}/classes/{classKey}`            |Class Detail View |Detailed view and actions for a single class.
|`/workspaces/{workspaceKey}/classes/{classKey}/edit`       |Class Editing     |Form for editing the metadata of an existing class.
|`/workspaces/{workspaceKey}/classes/{classKey}/clone`      |Class Cloning     |Dialog for replicating an existing class.
|`/workspaces/{workspaceKey}/classes/{classKey}/delete`     |Class Deletion    |Modal for confirming and executing the irreversible deletion of a class.
|`/workspaces/{workspaceKey}/classes/{classKey}/permissions`|Class Permissions |Modal for managing profiles (group-policy assignments) for a specific class.
|`/workspaces/{workspaceKey}/classes/import`                |Class Import      |Import of external class schemas.
|`/workspaces/{workspaceKey}/classes/export`                |Class Export      |Export of the current class schema for reuse or transfer.

## API Interfaces (REST Endpoints)

For programmatic interaction, third-party integration, and automation purposes, **KleeneStar** provides a standardized REST API for managing class definitions within a workspace. The interface adheres to REST principles and uses JSON as the data exchange format. Authentication and authorization are handled by **KleeneStar**. Standard HTTP status codes indicate the outcome of each request, including success, validation errors, permission issues, or missing resources.

The management of classes is handled via the following endpoints:

|Endpoint                                                                    |HTTP Method |Description
|----------------------------------------------------------------------------|------------|-------------------------------------------------------------------------------------------
|`/api/workspaces/{workspaceKey}/classes`                                    |GET         |Lists all classes defined in the specified workspace. Supports filtering and pagination.
|`/api/workspaces/{workspaceKey}/classes`                                    |POST        |Creates a new class. Requires at least a name.
|`/api/workspaces/{workspaceKey}/classes/{classKey}`                         |GET         |Retrieves detailed metadata and field definitions of a specific class.
|`/api/workspaces/{workspaceKey}/classes/{classKey}`                         |PUT         |Updates the metadata or field schema of an existing class. The class key remains immutable.
|`/api/workspaces/{workspaceKey}/classes/{classKey}`                         |DELETE      |Deletes a class definition. Requires confirmation and appropriate permissions.
|`/api/workspaces/{workspaceKey}/classes/{classKey}/clone`                   |POST        |Creates a duplicate of the specified class, including all fields and settings.
|`/api/workspaces/{workspaceKey}/classes/{classKey}/permissions`             |GET         |Lists all access profiles assigned to the class.
|`/api/workspaces/{workspaceKey}/classes/{classKey}/permissions`             |POST        |Assigns a group-policy profile to the class. Requires groupId and policyId in the request body.
|`/api/workspaces/{workspaceKey}/classes/{classKey}/permissions/{profileId}` |DELETE      |Removes a specific profile assignment from the class.
|`/api/workspaces/{workspaceKey}/classes/import`                             |POST        |Imports one or more class definitions from an external schema (e.g., JSON or YAML).
|`/api/workspaces/{workspaceKey}/classes/export`                             |GET         |Exports the current class schema for backup or reuse.

Standard error responses include:
- **400 Bad Request** - e.g., invalid schema or duplicate key
- **401 Unauthorized** - missing or invalid authentication
- **403 Forbidden** - insufficient permissions
- **404 Not Found** - class or workspace not found

Successful operations return:
- **201 Created** - for successful POST requests
- **200 OK** - for successful GET or PUT requests
- **204 No Content** - for successful DELETE operations

## Class Events

**KleeneStar**’s class management follows an event-driven architecture to ensure transparent and reactive communication of state changes across the system. Events are dispatched via the central WebExpress-EventManager, allowing modules, plugins, and external systems to respond to changes without tight coupling to the `ClassManager`.

The following events are emitted by the `ClassManager`:

|Event                    |Description
|-------------------------|-----------------------------------------------------------------------
|`ClassAdded`             |Emitted when a new class is created within a workspace.
|`ClassUpdated`           |Indicates changes to metadata or field definitions of an existing class.
|`ClassRemoved`           |Signals the permanent deletion of a class.
|`ClassCloned`            |Fired when a class is successfully duplicated.
|`ClassImported`          |Triggered upon importing external class definitions.
|`ClassExported`          |Triggered when a class schema is exported.
|`ClassPermissionChanged` |Indicates that access profiles for a class have been modified.

Each event includes a structured payload containing:
- Class key and associated workspace key
- Timestamp of the event
- Initiating user or module
- Action type and origin

These events are available both within the application and to connected subsystems, enabling reactive UI updates, audit logging, plugin hooks, and external synchronization.

## Permissions Model

In **KleeneStar**, class-level permissions are governed by context-specific profiles that link global groups to workspace-local policies. This model enables precise control over who can view, create, modify, or delete class definitions within a given workspace.

A profile assigns a global group a specific policy that applies only within the scope of a workspace. This allows the same group to hold different roles across multiple classes (for example, read-only access in one and full editing rights in another).

- **Assignment Logic:** A user inherits the permissions defined by a policy if they belong to a group that has an active profile (Group → Policy) in the workspace.
- **Granularity:** Policies can distinguish between viewing metadata, editing field definitions, managing permissions, or performing structural operations like cloning and importing.
- **Delegation:** Class administrators (e.g., via `class_admin_policy`) can manage profiles and adjust group-policy mappings for class-related actions.

The following table lists the granular permissions relevant to class management:

|Permission                 |Description
|---------------------------|----------------------------------------------------------------------------------
|`class_create`             |Allows the creation of new class definitions within a workspace.
|`class_read`               |Grants read access to class metadata and field definitions.
|`class_update`             |Authorizes the modification of an existing class, including fields and settings.
|`class_delete`             |Allows the permanent deletion of a class.
|`class_clone`              |Permits duplication of an existing class, including its structure and metadata.
|`class_import`             |Enables importing class schemas from external sources.
|`class_export`             |Allows exporting class definitions for reuse or backup.
|`class_manage_permissions` |Grants access to manage profiles and permission assignments for individual classes.

These permissions are bundled into logical policies to reflect typical roles and responsibilities:

|Policy                  |Description                                           |Included Permissions
|------------------------|------------------------------------------------------|-------------------------------------------------------------------------------------------------------
|`class_admin_policy`    |Full administrative control over class definitions.   |`class_read`, `class_update`, `class_delete`, `class_clone`, `class_import`, `class_export`, `class_manage_permissions`
|`class_edit_policy`     |Allows creation and modification of classes.          |`class_create`, `class_read`, `class_update`, `class_clone`
|`class_view_policy`     |Grants read-only access to class metadata and fields. |`class_read`
|`class_importer_policy` |Enables importing external class schemas.             |`class_import`
|`class_exporter_policy` |Enables exporting class definitions.                  |`class_export`

This permission model ensures that class management remains modular, secure, and adaptable to diverse organizational needs.

## Conclusion

The document "KleeneStar Class Management" outlines the conceptual framework for modeling and maintaining class definitions within modular workspaces. It defines key functional elements such as inheritance, abstraction, subclass composition, and blueprint-based field generation, and positions them within a flexible, UI-integrated lifecycle. As a high-level specification, the document intentionally leaves certain implementation aspects open. Topics like persistence mechanisms, concurrent schema updates, validation strategies, and version control are not defined in detail and must be addressed during implementation. Similarly, advanced features such as asynchronous import/export, audit integration, and plugin-driven schema augmentation are acknowledged but not elaborated.The focus lies on structural clarity, extensibility, and contributor empowerment. The proposed model supports recursive composition, dynamic UI rendering, and permission-aware operations, while remaining adaptable to diverse project needs. 
