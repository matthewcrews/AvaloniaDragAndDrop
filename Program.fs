﻿namespace CounterApp

open Avalonia

module Counter =

    open Avalonia.FuncUI.DSL
    open Avalonia.Controls
    open Avalonia.Layout

    type State = {
        SelectedButton: int
        ButtonLocations: Point[]
        PointerLocation: Point
    }
    let init() = {
        SelectedButton = -1
        ButtonLocations = 
            [|
                Point (10.0, 10.0)
                Point (10.0, 100.0)
            |]
        PointerLocation = Point (0.0, 0.0)
    }

    [<RequireQualifiedAccess>]
    type Msg =
    | Selected of buttonIdx: int * pointerLocation: Point
    | Deselected
    | Move of buttonIdx: int * newPointerPoint: Point

    let update (msg: Msg) (state: State) : State =
        match msg with
        | Msg.Selected (buttonIdx, position) ->
            { state with
                SelectedButton = buttonIdx
                PointerLocation = position }

        | Msg.Deselected ->
            { state with
                SelectedButton = -1 }

        | Msg.Move (buttonIdx, newPointerPoint) ->
            if state.SelectedButton = buttonIdx then
                let delta = newPointerPoint - state.PointerLocation
                let newState =
                    { state with
                        PointerLocation = newPointerPoint }
                newState.ButtonLocations[buttonIdx] <- state.ButtonLocations[buttonIdx] + delta
                newState
            else
                state

    let view (state: State) (dispatch) =
        Canvas.create [
            Canvas.name "DragArea"
            Canvas.children [
                for i in 0..state.ButtonLocations.Length - 1 do
                    let button = state.ButtonLocations[i]
                    Button.create [
                        Button.top button.Y
                        Button.left button.X
                        Button.width 100.0
                        Button.height 50.0
                        Button.background "Blue"
                        Button.onPointerPressed (fun e ->
                            let newPointerLocation = e.GetPosition null
                            Msg.Selected (i, newPointerLocation) |> dispatch)
                        Button.onPointerReleased (fun _ ->
                            Msg.Deselected |> dispatch)
                        Button.onPointerMoved (fun e ->
                            let newPointerLocation = e.GetPosition null
                            Msg.Move (i, newPointerLocation) |> dispatch)
                    ]
            ]
        ]

open Avalonia.Themes.Fluent
open Elmish
open Avalonia.FuncUI.Hosts
open Avalonia.FuncUI
open Avalonia.FuncUI.Elmish
open Avalonia.Controls.ApplicationLifetimes

type MainWindow() as this =
    inherit HostWindow()
    do
        base.Title <- "Counter Example"
        base.Height <- 400.0
        base.Width <- 400.0

        Elmish.Program.mkSimple Counter.init Counter.update Counter.view
        |> Program.withHost this
        |> Program.withConsoleTrace
        |> Program.run

type App() =
    inherit Application()

    override this.Initialize() =
        this.Styles.Add (FluentTheme())
        this.RequestedThemeVariant <- Styling.ThemeVariant.Dark

    override this.OnFrameworkInitializationCompleted() =
        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as desktopLifetime ->
            let mainWindow = MainWindow()
            desktopLifetime.MainWindow <- mainWindow
        | _ -> ()

module Program =

    [<EntryPoint>]
    let main(args: string[]) =
        AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
            .UseSkia()
            .StartWithClassicDesktopLifetime(args)
