namespace CounterApp

open Avalonia

module Counter =

    open Avalonia.FuncUI.DSL
    open Avalonia.Controls
    open Avalonia.Layout

    let notImplementedExn () =
        raise (System.NotImplementedException ())

    [<RequireQualifiedAccess>]
    type BlockType =
        | Buffer
        | Constraint
        // | Merge
        // | Split
        // | Conveyor
        // | Conversion

    type Block =
        {
            Location: Point
            Type: BlockType
            Width: float
            Height: float
        }

    type Blocks =
        {
            Count: int
            Locations: Point[]
            Types: BlockType[]
        }

    [<RequireQualifiedAccess>]
    type Selection =
        | Block of blockIdx: int * pointerPosition: Point
        | Input of blockIdx: int
        | Ouput of blockIdx: int

    [<RequireQualifiedAccess>]
    type Deselection =
        | Block of blockIdx: int
        | Input of blockIdx: int
        | Ouput of blockIdx: int

    [<RequireQualifiedAccess>]
    type PointerState =
        | Neutral
        | Dragging of blockIdx: int
        | ConnectingSource of blockIdx: int
        | ConnectingTarget of blockIdx: int

    [<RequireQualifiedAccess>]
    type Msg =
        | Selection of Selection
        | Deselection of Deselection
        | Move of buttonIdx: int * newPointerPoint: Point

    type State = {
        PointerState: PointerState
        Blocks: Blocks
        PointerLocation: Point
    }
    let init() = {
        PointerState = PointerState.Neutral
        Blocks =
            {
                Count = 2
                Locations = [|
                    Point (10.0, 10.0)
                    Point (10.0, 100.0)
                |]
                Types = [|
                    BlockType.Buffer
                    BlockType.Constraint
                |]
            }
        PointerLocation = Point (0.0, 0.0)
    }



    let update (msg: Msg) (state: State) : State =
        match msg with
        | Msg.Selection selection ->
            match state.PointerState with
            | PointerState.Neutral ->
                match selection with
                | Selection.Block (blockIdx, position) ->
                    { state with
                        PointerState = PointerState.Dragging blockIdx
                        PointerLocation = position }
                | _ ->
                    state

            | _ ->
                state

        | Msg.Deselection deselection ->

            match state.PointerState, deselection with
            | PointerState.Dragging draggingBlockIdx, _ ->
                { state with
                    PointerState = PointerState.Neutral }
            | _ ->
                    state

        | Msg.Move (buttonIdx, newPointerPoint) ->
            match state.PointerState with
            | PointerState.Dragging blockIdx ->
                if buttonIdx = blockIdx then
                    let delta = newPointerPoint - state.PointerLocation
                    let newState =
                        { state with
                            PointerLocation = newPointerPoint }
                    newState.Blocks.Locations[buttonIdx] <- state.Blocks.Locations[buttonIdx] + delta
                    newState
                else
                    state
            | _ ->
                    state

    module Views =

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
                                Msg.Move (blockIdx, newPointerLocation) |> dispatch)
                        ]
                        Button.create [
                            Button.width 10.0
                            Button.height 10.0
                            Button.background "Yellow"
                        ]
                    ]
                ]
                

        let constraint dispatch (location: Point) (blockIdx: int) =
                Button.create [
                    Button.top location.Y
                    Button.left location.X
                    Button.width 100.0
                    Button.height 50.0
                    Button.background "Red"
                    Button.onPointerPressed (fun e ->
                        let newPointerLocation = e.GetPosition null
                        Msg.Selection (Selection.Block (blockIdx, newPointerLocation)) |> dispatch)
                    Button.onPointerReleased (fun _ ->
                        Msg.Deselection (Deselection.Block blockIdx) |> dispatch)
                    Button.onPointerMoved (fun e ->
                        let newPointerLocation = e.GetPosition null
                        Msg.Move (blockIdx, newPointerLocation) |> dispatch)
                ]


    let view (state: State) (dispatch) =
        Canvas.create [
            Canvas.name "DragArea"
            Canvas.children [
                for blockIdx in 0..state.Blocks.Count - 1 do
                    let location = state.Blocks.Locations[blockIdx]
                    match state.Blocks.Types[blockIdx] with
                    | BlockType.Buffer ->
                        Views.buffer dispatch location blockIdx
                    | BlockType.Constraint ->
                        Views.constraint dispatch location blockIdx
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
