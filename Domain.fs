module AvaloniaDragAndDrop.Domain

#nowarn "46"

open Avalonia

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
    | Output of blockIdx: int

[<RequireQualifiedAccess>]
type Deselection =
    | Block of blockIdx: int
    | Input of blockIdx: int
    | Output of blockIdx: int

[<RequireQualifiedAccess>]
type PointerState =
    | Neutral
    | Dragging of blockIdx: int
    | ConnectingOutput of blockIdx: int
    | ConnectingInput of blockIdx: int

[<RequireQualifiedAccess>]
type Msg =
    | Escape
    | Selection of Selection
    | Deselection of Deselection
    | Move of newPointerPoint: Point

type State = {
    PointerState: PointerState
    Blocks: Blocks
    PointerLocation: Point
}

module State =

    let init () = {
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
        | Msg.Escape ->
            { state with
                PointerState = PointerState.Neutral }
        | Msg.Selection selection ->
            match state.PointerState with
            | PointerState.Neutral ->
                match selection with
                | Selection.Block (blockIdx, position) ->
                    { state with
                        PointerState = PointerState.Dragging blockIdx
                        PointerLocation = position }
                | Selection.Output blockIdx ->
                    { state with
                        PointerState = PointerState.ConnectingOutput blockIdx }

            | _ ->
                state

        | Msg.Deselection deselection ->

            match state.PointerState, deselection with
            | PointerState.Dragging draggingBlockIdx, _ ->
                { state with
                    PointerState = PointerState.Neutral }

            | PointerState.ConnectingOutput sourceBlockIdx, Deselection.Input targetBlockIdx ->
                    if sourceBlockIdx <> targetBlockIdx then
                        { state with
                            PointerState = PointerState.Neutral }
                    else
                        state
            | _ ->
                    state

        | Msg.Move newPointerPoint ->
            match state.PointerState with
            | PointerState.Dragging blockIdx ->
                let delta = newPointerPoint - state.PointerLocation
                let newState =
                    { state with
                        PointerLocation = newPointerPoint }
                newState.Blocks.Locations[blockIdx] <- state.Blocks.Locations[blockIdx] + delta
                newState

            | PointerState.ConnectingOutput _ ->
                { state with
                    PointerLocation = newPointerPoint }

            | _ ->
                    state
