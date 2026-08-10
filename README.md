# Revit API Modeless WPF Tools
---
## Features

**Modeless Window Execution**: Utilizes Revit's `ExternalEvent` pattern so WPF windows remain open while allowing seamless user interaction inside Revit.
  **Model Navigator**: 
  * Group elements dynamically by Category and Family Type.
  * Execute real-time operations: **Select**, **Isolate**, **Hide**, **Delete**, and **Override Surface/Cut Colors**.
  **Room Wizard**: 
  * Dynamically extract spatial element properties (**Name**, **Number**, **Area**, **Volume**).
  * Safely filters out unplaced/unbounded rooms and handles Revit spatial element casting.
  **Modern Material UI**: Styled using `MaterialDesignThemes` for a clean, dockable tool look and feel.
  **Clean Architecture**: Follows standard MVVM design pattern with decoupled commands and event handling.

---

## Technology Stack

* **Framework**: .NET Framework / C#
* **API Context**: Autodesk Revit API
* **UI Framework**: WPF (Windows Presentation Foundation)
* **Libraries**:
  * [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet) (MVVM pattern implementation)
  * [MaterialDesignInXAML](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit) (Modern WPF controls)

---

## 📁 Project Structure

```text
RevitAPIProject/
│
├── Application/
│   └── App.cs                     # Ribbon Tab, Panel, and PushButton registration
│
├── Command/
│   ├── ModelNavigatorCommand.cs   # ExternalCommand entry point for Model Navigator
│   └── RoomWizardCommand.cs       # ExternalCommand entry point for Room Wizard
│
├── Handlers/
│   └── RevitExternalEventHandler.cs  # Modeless thread synchronization (IExternalEventHandler)
│
├── Models/
│   ├── CategoryBind.cs            # WPF Binding model for categories
│   ├── ElementTypeBind.cs         # WPF Binding model for family types
│   └── RoomBiding.cs              # WPF Binding model for room data
│
├── ViewModels/
│   ├── ModelNavigatorViewModel.cs # ViewModel for Model Navigator
│   └── RoomWizardViewModel.cs     # ViewModel for Room Wizard
│
├── Views/
│   ├── ModelNavigatorView.xaml    # WPF Window UI for Model Navigator
│   └── RoomWizardView.xaml        # WPF Window UI for Room Wizard
│
└── Resources/                     # Button icons and image assets
