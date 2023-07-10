﻿module AvaloniaDragAndDrop.View

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

    module Line =

        let color = "Green"

    module Anchor =

        let width = 10.0
        let height = 10.0
        let color = "Yellow"


let inputAnchor dispatch blockId =
    Button.create [
        Button.width Dims.Anchor.width
        Button.height Dims.Anchor.height
        Button.background Dims.Anchor.color
        Button.onPointerReleased (fun e ->
            e.Handled <- true
            Msg.Selection (Selection.Input blockId)
            |> dispatch)
    ]

let outputAnchor dispatch blockId =
    Button.create [
        Button.width Dims.Anchor.width
        Button.height Dims.Anchor.height
        Button.background Dims.Anchor.color
        Button.onPointerReleased (fun e ->
            e.Handled <- true
            Msg.Selection (Selection.Output blockId)
            |> dispatch)
    ]


let buffer dispatch (location: Point) (name: string) (blockId: BlockId) =
    View.createWithKey $"Buffer{blockId}"
        DockPanel.create [
            DockPanel.top location.Y
            DockPanel.left location.X
            DockPanel.children [
                inputAnchor dispatch blockId
                Button.create [
                    Button.width Dims.Block.width
                    Button.height Dims.Block.height
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
                outputAnchor dispatch blockId
            ]
        ]

let constraint dispatch (location: Point) (name: string) (blockId: BlockId) =
    View.createWithKey $"Constraint{blockId}"
        DockPanel.create [
            DockPanel.top location.Y
            DockPanel.left location.X
            DockPanel.children [
                inputAnchor dispatch blockId
                Button.create [
                    Button.top location.Y
                    Button.left location.X
                    Button.content name
                    Button.width Dims.Block.width
                    Button.height Dims.Block.height
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
                outputAnchor dispatch blockId
            ]
        ]

let view (state: State) (dispatch) =
    let canvasName = "DiagramCanvas"
    Canvas.create [
        Canvas.name canvasName
        Canvas.background "DarkSlateGray"
        Canvas.onKeyUp (fun e ->
            e.Handled <- true
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
                    Line.startPoint (location + (Point (120.0, 25.0)))
                    Line.endPoint state.PointerLocation
                    Line.stroke Dims.Line.color
                    Line.strokeThickness 2.0
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
                let location = state.Blocks.Locations[blockIdx]
                Line.create [
                    Line.startPoint (location + (Point (0.0, 25.0)))
                    Line.endPoint state.PointerLocation
                    Line.stroke Dims.Line.color
                    Line.strokeThickness 2.0
                ]

            | _ -> ()

            for connection in state.Connections do
                let sourceLocation = state.Blocks.Locations[connection.Source]
                let targetLocation = state.Blocks.Locations[connection.Target]
                Line.create [
                    Line.startPoint (sourceLocation + (Point (120.0, 25.0)))
                    Line.endPoint (targetLocation + (Point (0.0, 25.0)))
                    Line.stroke "Green"
                    Line.strokeThickness 2.0
                ]

            // for blockIdx in 0..state.Blocks.Count - 1 do
            for KeyValue (blockId, location) in state.Blocks.Locations do
                match state.Blocks.Types[blockId] with
                | BlockType.Buffer bufferId ->
                    let bufferName = state.Blocks.Names[blockId]
                    buffer dispatch location bufferName blockId

                | BlockType.Constraint constraintId ->
                    let constraintName = state.Blocks.Names[blockId]
                    constraint dispatch location constraintName blockId

            match state.PointerState with
            | PointerState.AddingElement ->
                StackPanel.create [
                    StackPanel.orientation Orientation.Vertical
                    StackPanel.left state.AddElementMenuLocation.X
                    StackPanel.top state.AddElementMenuLocation.Y
                    StackPanel.children [
                        Button.create [
                            Button.content "Buffer"
                            Button.width 100.0
                            Button.height 30.0
                            Button.onClick (fun e ->
                                e.Handled <- true
                                Msg.AddBlock { AddBlockType = AddBlockType.Buffer; Location = state.AddElementMenuLocation } |> dispatch)
                        ]
                        Button.create [
                            Button.content "Constraint"
                            Button.width 100.0
                            Button.height 30.0
                            Button.onClick (fun e ->
                                e.Handled <- true
                                Msg.AddBlock { AddBlockType = AddBlockType.Constraint; Location = state.AddElementMenuLocation } |> dispatch)
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
                                    Msg.BlockChange (BlockChange.Buffer (BufferChange.Capacity (bufferId, float e)))
                                    |> dispatch)
                            ]
                            TextBlock.create [
                                TextBlock.text "Initial Volume"
                            ]
                            TextBox.create [
                                TextBox.text (string bufferAttr.InitialVolume)
                                TextBox.onTextChanged (fun e ->
                                    Msg.BlockChange (BlockChange.Buffer (BufferChange.InitialVolume (bufferId, float e)))
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
                                    Msg.BlockChange (BlockChange.Constraint (ConstraintChange.Limit (constraintId, float e)))
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
                                    Msg.BlockChange (BlockChange.Conversion (ConversionChange.Coefficient (conversionId, float e)))
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
                                    Msg.BlockChange (BlockChange.Conveyor (ConveyorChange.Height (conveyorId, float e)))
                                    |> dispatch)
                            ]
                            TextBlock.create [
                                TextBlock.text "Length"
                            ]
                            TextBox.create [
                                TextBox.text (string conveyorAttr.Length)
                                TextBox.onTextChanged (fun e ->
                                    Msg.BlockChange (BlockChange.Conveyor (ConveyorChange.Length (conveyorId, float e)))
                                    |> dispatch)
                            ]
                            TextBlock.create [
                                TextBlock.text "Max Velocity"
                            ]
                            TextBox.create [
                                TextBox.text (string conveyorAttr.MaxVelocity)
                                TextBox.onTextChanged (fun e ->
                                    Msg.BlockChange (BlockChange.Conveyor (ConveyorChange.MaxVelocity (conveyorId, float e)))
                                    |> dispatch)
                            ]
                    ]
                ]
        ]
    ]
