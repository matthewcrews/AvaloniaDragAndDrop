module AvaloniaDragAndDrop.View

open Avalonia
open Avalonia.Controls.Shapes
open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Avalonia.Input
open Avalonia.Interactivity
open Avalonia.Layout
open AvaloniaDragAndDrop.Domain


let addItemMenu =
    StackPanel.create [
        StackPanel.orientation Orientation.Vertical
        StackPanel.children [
            Button.create [
                Button.name "Buffer"
                Button.width 50.0
                Button.height 30.0
            ]
            Button.create [
                Button.name "Constraint"
                Button.width 50.0
                Button.height 30.0
            ]
        ]
    ]


let buffer dispatch (location: Point) (blockIdx: int) =
    DockPanel.create [
        DockPanel.top location.Y
        DockPanel.left location.X
        DockPanel.children [
            Button.create [
                Button.width 10.0
                Button.height 10.0
                Button.background "Yellow"
                Button.onPointerReleased (fun e ->
                    e.Handled <- true
                    Msg.Selection (Selection.Input blockIdx) |> dispatch)
            ]
            Button.create [
                Button.width 100.0
                Button.height 50.0
                Button.background "Blue"
                Button.onPointerPressed (fun e ->
                    e.Handled <- true
                    let newPointerLocation = e.GetPosition null
                    Msg.Selection (Selection.Block (blockIdx, newPointerLocation)) |> dispatch)
                Button.onPointerReleased (fun e ->
                    e.Handled <- true
                    Msg.Deselection (Deselection.Block blockIdx) |> dispatch)
                Button.onPointerMoved (fun e ->
                    e.Handled <- true
                    let newPointerLocation = e.GetPosition null
                    Msg.Move newPointerLocation |> dispatch)
            ]
            Button.create [
                Button.width 10.0
                Button.height 10.0
                Button.background "Yellow"
                Button.onPointerReleased (fun e ->
                    e.Handled <- true
                    Msg.Selection (Selection.Output blockIdx) |> dispatch)
                // Button.onPointerReleased (fun e ->
                //     e.Route <- RoutingStrategies.Direct)
            ]
        ]
    ]

let constraint dispatch (location: Point) (blockIdx: int) =
    DockPanel.create [
        DockPanel.top location.Y
        DockPanel.left location.X
        DockPanel.children [
            Button.create [
                Button.width 10.0
                Button.height 10.0
                Button.background "Yellow"
                Button.onPointerReleased (fun e ->
                    e.Handled <- true
                    Msg.Selection (Selection.Input blockIdx) |> dispatch)
            ]
            Button.create [
                Button.top location.Y
                Button.left location.X
                Button.width 100.0
                Button.height 50.0
                Button.background "Red"
                Button.onPointerPressed (fun e ->
                    e.Handled <- true
                    let newPointerLocation = e.GetPosition null
                    Msg.Selection (Selection.Block (blockIdx, newPointerLocation)) |> dispatch)
                Button.onPointerReleased (fun e ->
                    e.Handled <- true
                    Msg.Deselection (Deselection.Block blockIdx) |> dispatch)
                Button.onPointerMoved (fun e ->
                    e.Handled <- true
                    let newPointerLocation = e.GetPosition null
                    Msg.Move newPointerLocation |> dispatch)
            ]
            Button.create [
                Button.width 10.0
                Button.height 10.0
                Button.background "Yellow"
                Button.onPointerReleased (fun e ->
                    e.Handled <- true
                    Msg.Selection (Selection.Output blockIdx) |> dispatch)
            ]
        ]
    ]

let view (state: State) (dispatch) =
    let canvasName = "DiagramCanvas"
    Canvas.create [
        Canvas.name canvasName
        Canvas.background "DarkSlateGray"
        Canvas.onKeyUp (fun e ->
            if e.Key = Key.Escape then
                Msg.Escape |> dispatch)
        Canvas.onPointerMoved (fun e ->
            let source : Control = e.Source :?> Control
            if source.Name = canvasName then
                e.Handled <- true
                let newPointerLocation = e.GetPosition null
                Msg.Move newPointerLocation |> dispatch)
        Canvas.onPointerReleased (fun e ->
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


        Canvas.children [
            match state.PointerState with
            | PointerState.ConnectingOutput blockIdx ->
                let location = state.Blocks.Locations[blockIdx]
                Line.create [
                    Line.startPoint (location + (Point (100.0, 25.0)))
                    Line.endPoint state.PointerLocation
                    Line.stroke "Green"
                    Line.strokeThickness 2.0
                ]

            | PointerState.ConnectingInput blockIdx ->
                let location = state.Blocks.Locations[blockIdx]
                Line.create [
                    Line.startPoint location
                    Line.endPoint state.PointerLocation
                    Line.stroke "Green"
                    Line.strokeThickness 2.0
                ]

            | _ -> ()

            for connection in state.Connections do
                let sourceLocation = state.Blocks.Locations[connection.Source]
                let targetLocation = state.Blocks.Locations[connection.Target]

                Line.create [
                    Line.startPoint (sourceLocation + (Point (100.0, 25.0)))
                    Line.endPoint targetLocation
                    Line.stroke "Green"
                    Line.strokeThickness 2.0
                ]

            for blockIdx in 0..state.Blocks.Count - 1 do
                let location = state.Blocks.Locations[blockIdx]
                match state.Blocks.Types[blockIdx] with
                | BlockType.Buffer ->
                    buffer dispatch location blockIdx

                | BlockType.Constraint ->
                    constraint dispatch location blockIdx

            match state.PointerState with
            | PointerState.AddingElement ->
                StackPanel.create [
                    StackPanel.orientation Orientation.Vertical
                    StackPanel.left state.AddElementMenuLocation.X
                    StackPanel.top state.AddElementMenuLocation.Y
                    StackPanel.children [
                        Button.create [
                            Button.content "Buffer"
                            Button.width 50.0
                            Button.height 30.0
                            Button.onClick (fun e ->
                                e.Handled <- true
                                Msg.AddBlock { BlockType = BlockType.Buffer; Location = state.AddElementMenuLocation } |> dispatch)
                        ]
                        Button.create [
                            Button.content "Constraint"
                            Button.width 50.0
                            Button.height 30.0
                            Button.onClick (fun e ->
                                e.Handled <- true
                                Msg.AddBlock { BlockType = BlockType.Constraint; Location = state.AddElementMenuLocation } |> dispatch)
                        ]
                    ]
                ]

            | _ ->
                ()


        ]
    ]
