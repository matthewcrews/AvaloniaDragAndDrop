namespace CounterApp

open Avalonia

module Counter =

    open Avalonia.FuncUI.DSL
    open Avalonia.Controls
    open Avalonia.Layout

    type State = {
        IsSelected: bool
        X : float
        Y : float
    }
    let init() = {
        IsSelected = false
        X = 10.0
        Y = 10.0
    }

    type Msg =
    | Selected
    | Deselected
    | Move of x: float * y: float

    let update (msg: Msg) (state: State) : State =
        match msg with
        | Selected ->
            { state with IsSelected = true }
        | Deselected ->
            { state with IsSelected = false }
        | Move (x, y) ->
            if state.IsSelected then
                { state with X = x; Y = y }
            else
                state

    let view (state: State) (dispatch) =
        Button.create [
            Button.top state.Y
            Button.left state.X
            Button.width 10.0
            Button.height 10.0
            Button.background "Blue"
            Button.onClick (fun _ ->
                Msg.Selected |> dispatch)
            Button.onPointerReleased (fun _ ->
                Msg.Deselected |> dispatch)
            Button.onPointerMoved (fun e ->
                let delta = e.GetPosition null
                Msg.Move (delta.X, delta.Y) |> dispatch)
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
