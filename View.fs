module AvaloniaDragAndDrop.View

open System
open Avalonia
open Avalonia.Controls.Shapes
open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Avalonia.Input
open Avalonia.Layout
open Avalonia.Media
open AvaloniaDragAndDrop.Domain


module Dims =

    module Block =

        let width = 100.0
        let height = 50.0

    module Buffer =

        let color = "Blue"

    module Constraint =

        let color = "Red"

    module Conversion =

        let color = "Yellow"

    module Conveyor =

        let color = "Bisque"

    module Merge =

        let color = "Magenta"

    module Split =

        let color = "SeaGreen"

    module Line =

        let color = "Green"

    module Anchor =

        let width = 10.0
        let height = 10.0
        let color = "Yellow"


let inputAnchor dispatch (zoom: float) blockId =
    Button.create [
        Button.width (Dims.Anchor.width * zoom)
        Button.height (Dims.Anchor.height * zoom)
        Button.background Dims.Anchor.color
        Button.onPointerReleased (fun e ->
            e.Handled <- true
            Msg.Selection (Selection.Input blockId)
            |> dispatch)
    ]

let outputAnchor dispatch (zoom: float) blockId =
    Button.create [
        Button.width (Dims.Anchor.width * zoom)
        Button.height (Dims.Anchor.height * zoom)
        Button.background Dims.Anchor.color
        Button.onPointerReleased (fun e ->
            e.Handled <- true
            Msg.Selection (Selection.Output blockId)
            |> dispatch)
    ]


let buffer dispatch (origin: Point) (zoom: float) (location: Point) (name: string) (blockId: BlockId) =
    let location = zoom * location + origin
    View.createWithKey $"Buffer{blockId}"
        DockPanel.create [
            DockPanel.top location.Y
            DockPanel.left location.X
            DockPanel.children [
                inputAnchor dispatch zoom blockId
                Button.create [
                    Button.width (Dims.Block.width * zoom)
                    Button.height (Dims.Block.height * zoom)
                    Button.content name
                    Button.background Dims.Buffer.color
                    Button.onPointerPressed (fun e ->
                        e.Handled <- true
                        let newPointerLocation = e.GetPosition null
                        Msg.Selection (Selection.Block (blockId, newPointerLocation)) |> dispatch)
                    Button.onPointerReleased (fun e ->
                        e.Handled <- true
                        Msg.Deselection (Deselection.Block blockId) |> dispatch)
                    Button.onPointerMoved (fun e ->
                        e.Handled <- true
                        let newPointerLocation = e.GetPosition null
                        Msg.Move newPointerLocation |> dispatch)
                    Button.onDoubleTapped (fun e ->
                        e.Handled <- true
                        Msg.StartEditing blockId |> dispatch)
                ]
                outputAnchor dispatch zoom blockId
            ]
        ]

let constraint dispatch (origin: Point) (zoom: float) (location: Point) (name: string) (blockId: BlockId) =
    let location = zoom * location + origin
    View.createWithKey $"Constraint{blockId}"
        DockPanel.create [
            DockPanel.top location.Y
            DockPanel.left location.X
            DockPanel.children [
                inputAnchor dispatch zoom blockId
                Button.create [
                    Button.top location.Y
                    Button.left location.X
                    Button.content name
                    Button.width (Dims.Block.width * zoom)
                    Button.height (Dims.Block.height * zoom)
                    Button.background Dims.Constraint.color
                    Button.onPointerPressed (fun e ->
                        e.Handled <- true
                        let newPointerLocation = e.GetPosition null
                        Msg.Selection (Selection.Block (blockId, newPointerLocation)) |> dispatch)
                    Button.onPointerReleased (fun e ->
                        e.Handled <- true
                        Msg.Deselection (Deselection.Block blockId) |> dispatch)
                    Button.onPointerMoved (fun e ->
                        e.Handled <- true
                        let newPointerLocation = e.GetPosition null
                        Msg.Move newPointerLocation |> dispatch)
                    Button.onDoubleTapped (fun e ->
                        e.Handled <- true
                        Msg.StartEditing blockId |> dispatch)
                ]
                outputAnchor dispatch zoom blockId
            ]
        ]

let conversion dispatch (origin: Point) (zoom: float) (location: Point) (name: string) (blockId: BlockId) =
    let location = zoom * location + origin
    View.createWithKey $"Conversion{blockId}"
        DockPanel.create [
            DockPanel.top location.Y
            DockPanel.left location.X
            DockPanel.children [
                inputAnchor dispatch zoom blockId
                Button.create [
                    Button.top location.Y
                    Button.left location.X
                    Button.content name
                    Button.width (Dims.Block.width * zoom)
                    Button.height (Dims.Block.height * zoom)
                    Button.background Dims.Conversion.color
                    Button.onPointerPressed (fun e ->
                        e.Handled <- true
                        let newPointerLocation = e.GetPosition null
                        Msg.Selection (Selection.Block (blockId, newPointerLocation)) |> dispatch)
                    Button.onPointerReleased (fun e ->
                        e.Handled <- true
                        Msg.Deselection (Deselection.Block blockId) |> dispatch)
                    Button.onPointerMoved (fun e ->
                        e.Handled <- true
                        let newPointerLocation = e.GetPosition null
                        Msg.Move newPointerLocation |> dispatch)
                    Button.onDoubleTapped (fun e ->
                        e.Handled <- true
                        Msg.StartEditing blockId |> dispatch)
                ]
                outputAnchor dispatch zoom blockId
            ]
        ]

let conveyor dispatch (origin: Point) (zoom: float) (location: Point) (name: string) (blockId: BlockId) =
    let location = zoom * location + origin
    View.createWithKey $"Conversion{blockId}"
        DockPanel.create [
            DockPanel.top location.Y
            DockPanel.left location.X
            DockPanel.children [
                inputAnchor dispatch zoom blockId
                Button.create [
                    Button.top location.Y
                    Button.left location.X
                    Button.content name
                    Button.width (Dims.Block.width * zoom)
                    Button.height (Dims.Block.height * zoom)
                    Button.background Dims.Conveyor.color
                    Button.onPointerPressed (fun e ->
                        e.Handled <- true
                        let newPointerLocation = e.GetPosition null
                        Msg.Selection (Selection.Block (blockId, newPointerLocation)) |> dispatch)
                    Button.onPointerReleased (fun e ->
                        e.Handled <- true
                        Msg.Deselection (Deselection.Block blockId) |> dispatch)
                    Button.onPointerMoved (fun e ->
                        e.Handled <- true
                        let newPointerLocation = e.GetPosition null
                        Msg.Move newPointerLocation |> dispatch)
                    Button.onDoubleTapped (fun e ->
                        e.Handled <- true
                        Msg.StartEditing blockId |> dispatch)
                ]
                outputAnchor dispatch zoom blockId
            ]
        ]


let merge dispatch (origin: Point) (zoom: float) (location: Point) (name: string) (blockId: BlockId) =
    let location = zoom * location + origin
    View.createWithKey $"Conversion{blockId}"
        DockPanel.create [
            DockPanel.top location.Y
            DockPanel.left location.X
            DockPanel.children [
                inputAnchor dispatch zoom blockId
                Button.create [
                    Button.top location.Y
                    Button.left location.X
                    Button.content name
                    Button.width (Dims.Block.width * zoom)
                    Button.height (Dims.Block.height * zoom)
                    Button.background Dims.Merge.color
                    Button.onPointerPressed (fun e ->
                        e.Handled <- true
                        let newPointerLocation = e.GetPosition null
                        Msg.Selection (Selection.Block (blockId, newPointerLocation)) |> dispatch)
                    Button.onPointerReleased (fun e ->
                        e.Handled <- true
                        Msg.Deselection (Deselection.Block blockId) |> dispatch)
                    Button.onPointerMoved (fun e ->
                        e.Handled <- true
                        let newPointerLocation = e.GetPosition null
                        Msg.Move newPointerLocation |> dispatch)
                    Button.onDoubleTapped (fun e ->
                        e.Handled <- true
                        Msg.StartEditing blockId |> dispatch)
                ]
                outputAnchor dispatch zoom blockId
            ]
        ]


let split dispatch (origin: Point) (zoom: float) (location: Point) (name: string) (blockId: BlockId) =
    let location = zoom * location + origin
    View.createWithKey $"Conversion{blockId}"
        DockPanel.create [
            DockPanel.top location.Y
            DockPanel.left location.X
            DockPanel.children [
                inputAnchor dispatch zoom blockId
                Button.create [
                    Button.top location.Y
                    Button.left location.X
                    Button.content name
                    Button.width (Dims.Block.width * zoom)
                    Button.height (Dims.Block.height * zoom)
                    Button.background Dims.Split.color
                    Button.onPointerPressed (fun e ->
                        e.Handled <- true
                        let newPointerLocation = e.GetPosition null
                        Msg.Selection (Selection.Block (blockId, newPointerLocation)) |> dispatch)
                    Button.onPointerReleased (fun e ->
                        e.Handled <- true
                        Msg.Deselection (Deselection.Block blockId) |> dispatch)
                    Button.onPointerMoved (fun e ->
                        e.Handled <- true
                        let newPointerLocation = e.GetPosition null
                        Msg.Move newPointerLocation |> dispatch)
                    Button.onDoubleTapped (fun e ->
                        e.Handled <- true
                        Msg.StartEditing blockId |> dispatch)
                ]
                outputAnchor dispatch zoom blockId
            ]
        ]

let canvas (state: State) dispatch =
    let canvasName = "DiagramCanvas"
    Canvas.create [
        Canvas.dock Dock.Bottom
        Canvas.name canvasName
        Canvas.focusable true
        Canvas.background "DarkSlateGray"
        Canvas.onPointerMoved (fun e ->
            let source : Control = e.Source :?> Control
            if source.Name = canvasName then
                e.Handled <- true
                let newPointerLocation = e.GetPosition null
                Msg.Move newPointerLocation |> dispatch)
        Canvas.onPointerReleased (fun e ->
            if e.InitialPressMouseButton = MouseButton.Middle then
                e.Handled <- true
                Msg.PanningStopped |> dispatch
            else

                let source : Control = e.Source :?> Control
                if source.Name = canvasName then
                    e.Handled <- true
                    if e.InitialPressMouseButton = MouseButton.Right then
                        let newPointerLocation = e.GetPosition null
                        Msg.RequestAddItem newPointerLocation |> dispatch
                    elif e.InitialPressMouseButton = MouseButton.Left then
                        Msg.Escape |> dispatch
                    else
                        ()
            )
        Canvas.onPointerPressed (fun e ->
            let point = e.GetCurrentPoint null
            if point.Properties.IsMiddleButtonPressed then
                Msg.PanningStarted |> dispatch
            )

        Canvas.onPointerWheelChanged (fun e ->
            e.Handled <- true
            let zoomDelta = e.Delta.Y
            if zoomDelta > 0 then
                Msg.ZoomIn |> dispatch
            elif zoomDelta < 0 then
                Msg.ZoomOut |> dispatch)

        Canvas.children [
            match state.PointerState with
            | PointerState.ConnectingOutput blockIdx ->
                let location = state.Blocks.Locations[blockIdx] + (Point (120.0, 25.0))
                Line.create [
                    Line.startPoint (location * state.Zoom + state.WindowPosition)
                    Line.endPoint state.PointerPosition
                    Line.focusable true
                    Line.stroke Dims.Line.color
                    Line.strokeThickness 2.0
                    Line.onKeyDown (fun e ->
                        e.Handled <- true
                        if e.Key = Key.Escape then
                            Msg.Escape |> dispatch)
                    Line.onKeyUp (fun e ->
                        e.Handled <- true
                        if e.Key = Key.Escape then
                            Msg.Escape |> dispatch)
                ]
                // Drawing triangle
                // Canvas.create [
                //     Canvas.top 200.0
                //     Canvas.left 200.0
                //     Canvas.width 1.0
                //     Canvas.height 1.0
                //     Canvas.children [
                //         Path.create [
                //             Path.fill "black"
                //             Path.data "M8,5.14V19.14L19,12.14L8,5.14Z"
                //             Path.stretch Stretch.Uniform
                //         ]
                //     ]
                // ]


            | PointerState.ConnectingInput blockIdx ->
                let location = state.Blocks.Locations[blockIdx] + (Point (0.0, 25.0))
                Line.create [
                    Line.startPoint (location * state.Zoom + state.WindowPosition)
                    Line.endPoint state.PointerPosition
                    Line.stroke Dims.Line.color
                    Line.strokeThickness 2.0
                ]

            | _ -> ()

            for connection in state.Connections do
                let sourceLocation = state.Blocks.Locations[connection.Source] + (Point (120.0, 25.0))
                let targetLocation = state.Blocks.Locations[connection.Target] + (Point (0.0, 25.0))

                Line.create [
                    Line.startPoint (sourceLocation * state.Zoom + state.WindowPosition)
                    Line.endPoint (targetLocation * state.Zoom + state.WindowPosition)
                    Line.stroke "Green"
                    Line.strokeThickness 2.0
                ]

            // for blockIdx in 0..state.Blocks.Count - 1 do
            for KeyValue (blockId, location) in state.Blocks.Locations do
                let name = state.Blocks.Names[blockId]

                match state.Blocks.Types[blockId] with
                | BlockType.Buffer bufferId ->
                    buffer dispatch state.WindowPosition state.Zoom location name blockId

                | BlockType.Constraint constraintId ->
                    constraint dispatch state.WindowPosition state.Zoom location name blockId

                | BlockType.Conversion conversionId ->
                    conversion dispatch state.WindowPosition state.Zoom location name blockId

                | BlockType.Conveyor conveyorId ->
                    conveyor dispatch state.WindowPosition state.Zoom location name blockId

                | BlockType.Merge mergeId ->
                    merge dispatch state.WindowPosition state.Zoom location name blockId

                | BlockType.Split splitId ->
                    split dispatch state.WindowPosition state.Zoom location name blockId


            match state.PointerState with
            | PointerState.AddingElement ->
                let location = state.AddElementMenuLocation
                let addLocation = (state.PointerPosition - state.WindowPosition) / state.Zoom
                StackPanel.create [
                    StackPanel.orientation Orientation.Vertical
                    StackPanel.left location.X
                    StackPanel.top location.Y
                    StackPanel.children [
                        Button.create [
                            Button.content "Buffer"
                            Button.width Dims.Block.width
                            Button.height Dims.Block.height
                            Button.onClick (fun e ->
                                e.Handled <- true
                                Msg.Cmd (Cmd.AddBlock { AddBlockType = AddBlockType.Buffer; Location = addLocation })
                                |> dispatch)
                        ]
                        Button.create [
                            Button.content "Constraint"
                            Button.width Dims.Block.width
                            Button.height Dims.Block.height
                            Button.onClick (fun e ->
                                e.Handled <- true
                                Msg.Cmd  (Cmd.AddBlock { AddBlockType = AddBlockType.Constraint; Location = addLocation })
                                |> dispatch)
                        ]
                        Button.create [
                            Button.content "Conversion"
                            Button.width Dims.Block.width
                            Button.height Dims.Block.height
                            Button.onClick (fun e ->
                                e.Handled <- true
                                Msg.Cmd (Cmd.AddBlock { AddBlockType = AddBlockType.Conversion; Location = addLocation })
                                |> dispatch)
                        ]
                        Button.create [
                            Button.content "Conveyor"
                            Button.width Dims.Block.width
                            Button.height Dims.Block.height
                            Button.onClick (fun e ->
                                e.Handled <- true
                                Msg.Cmd (Cmd.AddBlock { AddBlockType = AddBlockType.Conveyor; Location = addLocation })
                                |> dispatch)
                        ]
                        Button.create [
                            Button.content "Merge"
                            Button.width Dims.Block.width
                            Button.height Dims.Block.height
                            Button.onClick (fun e ->
                                e.Handled <- true
                                Msg.Cmd (Cmd.AddBlock { AddBlockType = AddBlockType.Merge; Location = addLocation })
                                |> dispatch)
                        ]
                        Button.create [
                            Button.content "Split"
                            Button.width Dims.Block.width
                            Button.height Dims.Block.height
                            Button.onClick (fun e ->
                                e.Handled <- true
                                Msg.Cmd (Cmd.AddBlock { AddBlockType = AddBlockType.Split; Location = addLocation })
                                |> dispatch)
                        ]
                    ]
                ]

            | _ ->
                ()

            match state.EditBlock with
            | ValueNone -> ()
            | ValueSome blockIdx ->
                let location = state.Blocks.Locations[blockIdx]
                StackPanel.create [
                    StackPanel.background "Black"
                    StackPanel.top location.Y
                    StackPanel.left location.X
                    StackPanel.orientation Orientation.Vertical
                    StackPanel.children [
                        match state.Blocks.Types[blockIdx] with
                        | BlockType.Buffer bufferId ->
                            let bufferAttr = state.Blocks.Attributes.Buffer[bufferId]
                            TextBlock.create [
                                TextBlock.text "Capacity"
                            ]
                            TextBox.create [
                                TextBox.text (string bufferAttr.Capacity)
                                TextBox.onTextChanged (fun e ->
                                    Msg.Cmd (Cmd.ChangeBlock (BlockChangePayload.Buffer (BufferChange.Capacity (bufferId, float e))))
                                    |> dispatch)
                            ]
                            TextBlock.create [
                                TextBlock.text "Initial Volume"
                            ]
                            TextBox.create [
                                TextBox.text (string bufferAttr.InitialVolume)
                                TextBox.onTextChanged (fun e ->
                                    Msg.Cmd (Cmd.ChangeBlock (BlockChangePayload.Buffer (BufferChange.InitialVolume (bufferId, float e))))
                                    |> dispatch)
                            ]

                        | BlockType.Constraint constraintId ->
                            let constraintAttr = state.Blocks.Attributes.Constraints[constraintId]
                            TextBlock.create [
                                TextBlock.text "Limit"
                            ]
                            TextBox.create [
                                TextBox.text (string constraintAttr.Limit)
                                TextBox.onTextChanged (fun e ->
                                    Msg.Cmd (Cmd.ChangeBlock (BlockChangePayload.Constraint (ConstraintChange.Limit (constraintId, float e))))
                                    |> dispatch)
                            ]

                        | BlockType.Conversion conversionId ->
                            let conversionAttr = state.Blocks.Attributes.Conversions[conversionId]
                            TextBlock.create [
                                TextBlock.text "Coefficient"
                            ]
                            TextBox.create [
                                TextBox.text (string conversionAttr.Coefficient)
                                TextBox.onTextChanged (fun e ->
                                    Msg.Cmd (Cmd.ChangeBlock (BlockChangePayload.Conversion (ConversionChange.Coefficient (conversionId, float e))))
                                    |> dispatch)
                            ]

                        | BlockType.Conveyor conveyorId ->
                            let conveyorAttr = state.Blocks.Attributes.Conveyors[conveyorId]
                            TextBlock.create [
                                TextBlock.text "Height"
                            ]
                            TextBox.create [
                                TextBox.text (string conveyorAttr.Height)
                                TextBox.onTextChanged (fun e ->
                                    Msg.Cmd (Cmd.ChangeBlock (BlockChangePayload.Conveyor (ConveyorChange.Height (conveyorId, float e))))
                                    |> dispatch)
                            ]
                            TextBlock.create [
                                TextBlock.text "Length"
                            ]
                            TextBox.create [
                                TextBox.text (string conveyorAttr.Length)
                                TextBox.onTextChanged (fun e ->
                                    Msg.Cmd (Cmd.ChangeBlock (BlockChangePayload.Conveyor (ConveyorChange.Length (conveyorId, float e))))
                                    |> dispatch)
                            ]
                            TextBlock.create [
                                TextBlock.text "Max Velocity"
                            ]
                            TextBox.create [
                                TextBox.text (string conveyorAttr.MaxVelocity)
                                TextBox.onTextChanged (fun e ->
                                    Msg.Cmd (Cmd.ChangeBlock (BlockChangePayload.Conveyor (ConveyorChange.MaxVelocity (conveyorId, float e))))
                                    |> dispatch)
                            ]
                    ]
                ]
        ]
    ]

let menu (state: State) dispatch =
    Menu.create [
        Menu.dock Dock.Top
        Menu.horizontalAlignment HorizontalAlignment.Left
        Menu.viewItems [
            MenuItem.create [
                MenuItem.header "File"
                MenuItem.viewItems [
                    MenuItem.create [
                        MenuItem.header "Open"
                        MenuItem.onClick (fun e ->
                            Msg.OpenLoadScreen |> dispatch)
                    ]
                    MenuItem.create [
                        MenuItem.header "Save"
                        MenuItem.onClick (fun e ->
                            Msg.SaveRequested |> dispatch)
                    ]
                    MenuItem.create [
                        MenuItem.header "Save as"
                    ]
                    MenuItem.create [
                        MenuItem.header "Exit"
                    ]
                ]
            ]
            MenuItem.create [
                MenuItem.header "Edit"
                MenuItem.viewItems [
                    MenuItem.create [
                        MenuItem.header "Undo"
                    ]
                    MenuItem.create [
                        MenuItem.header "Redo"
                    ]
                    MenuItem.create [
                        MenuItem.header "Save as"
                    ]
                    MenuItem.create [
                        MenuItem.header "Exit"
                    ]
                ]
            ]
            MenuItem.create [
                MenuItem.header "View"
            ]
        ]
    ]

let save (state: State) dispatch =
    let outputDirectory =
        state.OutputDirectory
        |> Option.defaultValue (Environment.GetFolderPath(Environment.SpecialFolder.UserProfile))

    let outputDirectoryFiles =
        System.IO.Directory.GetFiles outputDirectory
        |> Array.map System.IO.Path.GetFileName

    StackPanel.create [
        StackPanel.orientation Orientation.Vertical
        StackPanel.children [
            TextBlock.create [
                TextBlock.text "File name:"
            ]
            TextBox.create [
                TextBox.text (state.FileName |> Option.defaultValue "")
                TextBox.onTextChanged (fun e ->
                    Msg.SaveFileNameChanged e |> dispatch)
            ]
            Button.create [
                Button.content "Save"
                Button.onClick (fun e ->
                    Msg.SaveRequested |> dispatch)
            ]
            TextBlock.create [
                TextBlock.text "Directory:"
            ]
            TextBlock.create [
                TextBlock.text outputDirectory
            ]
            TextBlock.create [
                TextBlock.text "Files:"
            ]
            for outputDirectoryFile in outputDirectoryFiles do
                TextBlock.create [
                    TextBlock.text outputDirectoryFile
                ]
        ]
    ]

let load (state: State) dispatch =
    let outputDirectory =
        state.OutputDirectory
        |> Option.defaultValue (Environment.GetFolderPath(Environment.SpecialFolder.UserProfile))

    let outputDirectoryFiles =
        System.IO.Directory.GetFiles outputDirectory
        |> Array.map System.IO.Path.GetFileName
        |> Array.filter (fun s ->
            s.EndsWith ".aidos")

    StackPanel.create [
        StackPanel.orientation Orientation.Vertical
        StackPanel.children [
            TextBlock.create [
                TextBlock.text "Directory:"
            ]
            TextBlock.create [
                TextBlock.text outputDirectory
            ]
            TextBlock.create [
                TextBlock.text "Files:"
            ]
            for outputDirectoryFile in outputDirectoryFiles do
                Button.create [
                    Button.content outputDirectoryFile
                    Button.onClick (fun e ->
                        Msg.LoadFile outputDirectoryFile |> dispatch)
                ]
        ]
    ]


let view (state: State) (dispatch) =
    DockPanel.create [
        DockPanel.children [
            menu state dispatch
            match state.Window with
            | Window.Editor ->
                canvas state dispatch
            | Window.Save ->
                save state dispatch
            | Window.Load ->
                load state dispatch
        ]
    ]
