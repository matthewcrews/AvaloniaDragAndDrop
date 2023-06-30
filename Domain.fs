﻿module AvaloniaDragAndDrop.Domain

#nowarn "46"

open Avalonia
open Microsoft.FSharp.Core

let notImplementedExn () =
    raise (System.NotImplementedException ())

module Array =

    let add elem (array: _[]) =
        let newArray = Array.zeroCreate (array.Length + 1)
        array.CopyTo (newArray, 0)
        newArray[array.Length] <- elem
        newArray


type BufferAttr =
    {
        Capacity: float
        InitialVolume: float
    }
    static member Zero =
        {
            Capacity = 100.0
            InitialVolume = 100.0
        }


[<RequireQualifiedAccess>]
type BlockType =
    | Buffer of bufferId: int
    | Constraint of constraintId: int
    // | Merge
    // | Split
    // | Conveyor
    // | Conversion

type Block =
    {
        Location: Point
        Type: BlockType
    }

type ConstraintAttr =
    {
        Limit: float
    }

type Blocks =
    {
        Count: int
        Locations: Point[]
        Types: BlockType[]
        BufferAttrs: BufferAttr[]
        ConstraintAttrs: ConstraintAttr[]
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


type AddBlockType =
    | Buffer
    | Constraint

type AddBlockPayload = {
    Location: Point
    AddBlockType: AddBlockType
}

type BufferChange =
    | InitialVolume of bufferId: int * newInitialVolume: float
    | Capacity of bufferId: int * newCapacity: float

type ConstraintChange =
    | Limit of constraintId: int * newLimit: float

type BlockChange =
    | Buffer of BufferChange
    | Constraint of ConstraintChange


[<RequireQualifiedAccess>]
type Msg =
    | Escape
    | Selection of Selection
    | Deselection of Deselection
    | Move of newPointerPoint: Point
    | RequestAddItem of location: Point
    | AddBlock of addBlockPayload: AddBlockPayload
    | StartEditing of blockIdx: int
    | BlockChange of BlockChange

[<Struct>]
type Connection = {
    Source: int
    Target: int
}

type NextIds =
    {
        Buffer: int
        Constraint: int
    }
    static member Zero =
        {
            Buffer = 0
            Constraint = 0
        }
    static member nextBufferId nextIds =
        nextIds.Buffer + 1, { nextIds with Buffer = nextIds.Buffer + 1 }

    static member nextConstraintId nextIds =
        nextIds.Constraint + 1, { nextIds with Constraint =  nextIds.Constraint + 1 }

type State = {
    NextIds: NextIds
    PointerState: PointerState
    Blocks: Blocks
    PointerLocation: Point
    Connections: Set<Connection>
    AddElementMenuLocation: Point
    EditBlock: int option
}

module State =

    let init () = {
        NextIds = NextIds.Zero
        PointerState = PointerState.Neutral
        Blocks =
            {
                Count = 2
                Locations = [|
                    Point (10.0, 10.0)
                    Point (10.0, 100.0)
                |]
                Types = [|
                    BlockType.Buffer 0
                    BlockType.Constraint 0
                |]
                BufferAttrs = [|
                    {
                        Capacity = 100.0
                        InitialVolume = 100.0
                    }
                |]
                ConstraintAttrs = [|
                    {
                        Limit = 10.0
                    }
                |]
            }
        Connections = Set.empty
        PointerLocation = Point (0.0, 0.0)
        AddElementMenuLocation = Point (0.0, 0.0)
        EditBlock = None
    }

    let update (msg: Msg) (state: State) : State =
        match msg with
        | Msg.Escape ->
            { state with
                PointerState = PointerState.Neutral
                EditBlock = None }

        | Msg.StartEditing blockIdx ->
            { state with
                EditBlock = Some blockIdx }

        | Msg.BlockChange blockChange ->
            match blockChange with
            | BlockChange.Buffer bufferChange ->
                match bufferChange with
                | BufferChange.InitialVolume(bufferId, newInitialVolume) ->
                    state.Blocks.BufferAttrs[bufferId] <- { state.Blocks.BufferAttrs[bufferId] with InitialVolume = newInitialVolume }
                    state

                | BufferChange.Capacity(bufferId, newCapacity) ->
                    state.Blocks.BufferAttrs[bufferId] <- { state.Blocks.BufferAttrs[bufferId] with Capacity =  newCapacity }
                    state

            | BlockChange.Constraint constraintChange ->
                match constraintChange with
                | ConstraintChange.Limit(constraintId, newLimit) ->
                    state.Blocks.ConstraintAttrs[constraintId] <- { state.Blocks.ConstraintAttrs[constraintId] with Limit = newLimit }
                    state

        | Msg.AddBlock addBlockPayload ->
            match addBlockPayload.AddBlockType with
            | AddBlockType.Buffer ->
                let bufferId, nextIds = NextIds.nextBufferId state.NextIds
                { state with
                    NextIds = nextIds
                    PointerState = PointerState.Neutral
                    Blocks = { state.Blocks with
                                Count = state.Blocks.Count + 1
                                Types = Array.add (BlockType.Buffer bufferId) state.Blocks.Types
                                Locations = Array.add addBlockPayload.Location state.Blocks.Locations } }
            | AddBlockType.Constraint ->
                let constraintId, nextIds = NextIds.nextBufferId state.NextIds
                { state with
                    NextIds = nextIds
                    PointerState = PointerState.Neutral
                    Blocks = { state.Blocks with
                                Count = state.Blocks.Count + 1
                                Types = Array.add (BlockType.Constraint constraintId) state.Blocks.Types
                                Locations = Array.add addBlockPayload.Location state.Blocks.Locations } }


        | Msg.RequestAddItem pointerLocation ->
            { state with
                PointerState = PointerState.AddingElement
                AddElementMenuLocation = pointerLocation }

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
