module AvaloniaDragAndDrop.View

open Avalonia
open Avalonia.Controls.Shapes
open Avalonia.Controls
open Avalonia.Layout
open Avalonia.FuncUI.DSL
open AvaloniaDragAndDrop.Domain

let buffer dispatch (location: Point) (blockIdx: int) =
    DockPanel.create [
        DockPanel.top location.Y
        DockPanel.left location.X
        DockPanel.children [
            Button.create [
                Button.width 10.0
                Button.height 10.0
                Button.background "Yellow"
            ]
            Button.create [
                Button.width 100.0
                Button.height 50.0
                Button.background "Blue"
                Button.onPointerPressed (fun e ->
                    let newPointerLocation = e.GetPosition null
                    Msg.Selection (Selection.Block (blockIdx, newPointerLocation)) |> dispatch)
                Button.onPointerReleased (fun _ ->
                    Msg.Deselection (Deselection.Block blockIdx) |> dispatch)
                Button.onPointerMoved (fun e ->
                    let newPointerLocation = e.GetPosition null
                    Msg.Move newPointerLocation |> dispatch)
            ]
            Button.create [
                Button.width 10.0
                Button.height 10.0
                Button.background "Yellow"
                Button.onPointerPressed (fun _ ->
                    Msg.Selection (Selection.Output blockIdx) |> dispatch)
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
                Button.onPointerPressed (fun _ ->
                    Msg.Selection (Selection.Output blockIdx) |> dispatch)
            ]
            Button.create [
                Button.top location.Y
                Button.left location.X
                Button.width 100.0
                Button.height 50.0
                Button.background "Red"
                Button.onPointerPressed (fun e ->
                    // let pointer = new Input.Pointer(0, Input.PointerType.Mouse, true)
                    // pointer.Capture (pointer :> Input.IInputElement)
                    let newPointerLocation = e.GetPosition null
                    Msg.Selection (Selection.Block (blockIdx, newPointerLocation)) |> dispatch)
                Button.onPointerReleased (fun _ ->
                    Msg.Deselection (Deselection.Block blockIdx) |> dispatch)
                Button.onPointerMoved (fun e ->
                    let newPointerLocation = e.GetPosition null
                    Msg.Move newPointerLocation |> dispatch)
            ]
            Button.create [
                Button.width 10.0
                Button.height 10.0
                Button.background "Yellow"
                Button.onPointerPressed (fun _ ->
                    Msg.Selection (Selection.Output blockIdx) |> dispatch)
            ]
        ]
    ]

let view (state: State) (dispatch) =
    Canvas.create [
        Canvas.background "DarkSlateGray"
        Canvas.onKeyUp (fun e ->
            if e.Key = Input.Key.Escape then
                Msg.Escape |> dispatch)
        Canvas.onPointerMoved (fun e ->
            let newPointerLocation = e.GetPosition null
            Msg.Move newPointerLocation |> dispatch)
        Canvas.children [
            for blockIdx in 0..state.Blocks.Count - 1 do
                let location = state.Blocks.Locations[blockIdx]
                match state.Blocks.Types[blockIdx] with
                | BlockType.Buffer ->
                    buffer dispatch location blockIdx

                | BlockType.Constraint ->
                    constraint dispatch location blockIdx

            match state.PointerState with
            | PointerState.ConnectingOutput blockIdx ->
                let location = state.Blocks.Locations[blockIdx]
                Line.create [
                    Line.startPoint (location + (Point (100.0, 25.0)))
                    Line.endPoint state.PointerLocation
                    Line.stroke "Green"
                    Line.strokeThickness 2.0
                ]

            | _ -> ()

        ]
    ]
