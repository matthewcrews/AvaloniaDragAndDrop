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
    // | Input of blockIdx: int
    // | Output of blockIdx: int

[<RequireQualifiedAccess>]
type PointerState =
    | Neutral
    | Dragging of blockIdx: int
    | ConnectingOutput of blockIdx: int
    | ConnectingInput of blockIdx: int
    | AddingElement

[<RequireQualifiedAccess>]
type Msg =
    | Escape
    | Selection of Selection
    | Deselection of Deselection
    | Move of newPointerPoint: Point
    | RequestAddItem of location: Point

[<Struct>]
type Connection = {
    Source: int
    Target: int
}

type State = {
    PointerState: PointerState
    Blocks: Blocks
    PointerLocation: Point
    Connections: Set<Connection>
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
        Connections = Set.empty
        PointerLocation = Point (0.0, 0.0)
    }

    let update (msg: Msg) (state: State) : State =
        match msg with
        | Msg.Escape ->
            { state with
                PointerState = PointerState.Neutral }

        | Msg.RequestAddItem pointerLocation ->
            { state with
                PointerState = PointerState.AddingElement
                PointerLocation = pointerLocation }

        | Msg.Selection selection ->
            match state.PointerState with
            | PointerState.Neutral ->
                match selection with
                | Selection.Block (blockIdx, position) ->
                    { state with
                        PointerState = PointerState.Dragging blockIdx
                        PointerLocation = position }
                | Selection.Output blockIdx
                | Selection.Input blockIdx ->
                    { state with
                        PointerState = PointerState.ConnectingOutput blockIdx }

            | PointerState.ConnectingOutput sourceBlockIdx ->
                match selection with
                | Selection.Input blockIdx ->
                    if sourceBlockIdx <> blockIdx then
                        { state with
                            PointerState = PointerState.Neutral
                            Connections = state.Connections.Add { Source = sourceBlockIdx; Target = blockIdx } }
                    else
                        state

                | _ ->
                    state

            | PointerState.ConnectingInput inputBlockIdx ->
                match selection with
                | Selection.Output blockIdx ->
                    if inputBlockIdx <> blockIdx then
                        { state with
                            PointerState = PointerState.Neutral
                            Connections = state.Connections.Add { Source = blockIdx; Target = inputBlockIdx } }
                    else
                        state
                | _ ->
                    state
            | _ ->
                state

        | Msg.Deselection deselection ->

            match state.PointerState, deselection with
            | PointerState.Dragging _, _ ->
                { state with
                    PointerState = PointerState.Neutral }

            // | PointerState.ConnectingOutput sourceBlockIdx, Deselection.Input targetBlockIdx ->
            //         if sourceBlockIdx <> targetBlockIdx then
            //             { state with
            //                 PointerState = PointerState.Neutral }
            //         else
            //             state
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

            | _ ->
                { state with
                    PointerLocation = newPointerPoint }
