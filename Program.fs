namespace CounterApp

#nowarn "46"

open Avalonia
open Avalonia.Controls
open Avalonia.Input
open Avalonia.Themes.Fluent
open Elmish
open Avalonia.FuncUI.Hosts
open Avalonia.FuncUI
open Avalonia.FuncUI.Elmish
open Avalonia.Controls.ApplicationLifetimes
open AvaloniaDragAndDrop
open AvaloniaDragAndDrop.Domain

type MainWindow() as this =
    inherit HostWindow()
    do
        let topLevel = TopLevel.GetTopLevel this
        base.Title <- "Counter Example"
        base.Height <- 400.0
        base.Width <- 400.0

        let subscriptions (state: State) : Sub<Msg> =
            let keyDownSubscription (dispatch: Msg -> unit) =
                let topLevel = TopLevel.GetTopLevel this
                topLevel.KeyDown.Subscribe(fun e ->
                    if e.Key = Key.Escape then
                        dispatch Msg.Escape
                )

            [
                [ nameof keyDownSubscription ], keyDownSubscription
            ]

        Elmish.Program.mkSimple State.init State.update View.view
        |> Program.withHost this
        |> Program.withSubscription subscriptions
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
