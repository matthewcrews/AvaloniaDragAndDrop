module AvaloniaDragAndDrop.Domain

#nowarn "46"

open System.Collections.Generic
open Avalonia

let notImplementedExn () =
    raise (System.NotImplementedException ())

[<RequireQualifiedAccess>]
module Units =

    [<Measure>] type BlockId
    [<Measure>] type BufferId
    [<Measure>] type ConstraintId
    [<Measure>] type ConversionId
    [<Measure>] type ConveyorId
    [<Measure>] type MergeId
    [<Measure>] type SplitId
    [<Measure>] type InterruptId


type BlockId = int<Units.BlockId>
type BufferId = int<Units.BufferId>
type ConstraintId = int<Units.ConstraintId>
type ConversionId = int<Units.ConversionId>
type ConveyorId = int<Units.ConveyorId>
type MergeId = int<Units.MergeId>
type SplitId = int<Units.SplitId>
type InterruptId = int<Units.InterruptId>


type Distribution =
    | Weibull of float * float

type Interrupt =
    {
        Name: string
        Distribution: Distribution
    }

module Attributes =

    [<Struct>]
    type Buffer =
        {
            Capacity: float
            InitialVolume: float
        }
        static member Zero =
            {
                Capacity = 100.0
                InitialVolume = 100.0
            }

    [<Struct>]
    type Constraint =
        {
            Limit: float
        }

    [<RequireQualifiedAccess>]
    type SplitType =
        | Priority of BlockId[]
        | Proportional of (float * int)[]

    [<Struct>]
    type Split =
        {
            Type: SplitType
        }

    [<RequireQualifiedAccess>]
    type MergeType =
        | Priority of int[]
        | Proportional of (float *  int)[]

    [<Struct>]
    type Merge =
        {
            Type: MergeType
        }

    [<Struct>]
    type Conversion =
        {
            Coefficient: float
        }

    [<Struct>]
    type Conveyor =
        {
            Length: float
            Height: float
            MaxVelocity: float
        }


[<Struct>]
type Attributes =
    {
        Buffer: Dictionary<BufferId, Attributes.Buffer>
        Constraints: Dictionary<ConstraintId, Attributes.Constraint>
        Merges: Dictionary<MergeId, Attributes.Merge>
        Splits: Dictionary<SplitId, Attributes.Split>
        Conversions: Dictionary<ConversionId, Attributes.Conversion>
        Conveyors: Dictionary<ConveyorId, Attributes.Conveyor>
    }
    static member init () =
        {
            Buffer = Dictionary()
            Constraints = Dictionary()
            Merges = Dictionary()
            Splits = Dictionary()
            Conversions = Dictionary()
            Conveyors = Dictionary()
        }

[<RequireQualifiedAccess>]
type BlockType =
    | Buffer of bufferId: BufferId
    | Constraint of constraintId: ConstraintId
    | Merge of mergeId: MergeId
    | Split of splitId: SplitId
    | Conveyor of conveyorId: ConveyorId
    | Conversion of conversionId: ConversionId

[<Struct>]
type Block =
    {
        Location: Point
        Type: BlockType
    }

[<Struct>]
type Blocks =
    {
        Names: Dictionary<BlockId, string>
        Locations: Dictionary<BlockId, Point>
        Types: Dictionary<BlockId, BlockType>
        Attributes: Attributes
    }

[<RequireQualifiedAccess>]
type Selection =
    | Block of blockId: BlockId * pointerPosition: Point
    | Input of blockId: BlockId
    | Output of blockId: BlockId

[<RequireQualifiedAccess>]
type Deselection =
    | Block of blockId: BlockId
    // | Input of blockIdx: int
    // | Output of blockIdx: int

[<RequireQualifiedAccess>]
type PointerState =
    | Neutral
    | Dragging of blockId: BlockId
    | ConnectingOutput of blockId: BlockId
    | ConnectingInput of blockId: BlockId
    | AddingElement


type AddBlockType =
    | Buffer
    | Constraint
    | Conversion
    | Conveyor
    | Merge
    | Split

type AddBlockPayload = {
    Location: Point
    AddBlockType: AddBlockType
}

type BufferChange =
    | InitialVolume of bufferId: BufferId * newInitialVolume: float
    | Capacity of bufferId: BufferId * newCapacity: float

type ConstraintChange =
    | Limit of constraintId: ConstraintId * newLimit: float

type ConversionChange =
    | Coefficient of conversionId: ConversionId * newCoefficient: float

type ConveyorChange =
    | Height of conveyorId: ConveyorId * newHeight: float
    | Length of conveyorId: ConveyorId * newLength: float
    | MaxVelocity of conveyorId: ConveyorId * newMaxVelocity: float

type BlockChange =
    | Buffer of BufferChange
    | Constraint of ConstraintChange
    | Conversion of ConversionChange
    | Conveyor of ConveyorChange


[<RequireQualifiedAccess>]
type Msg =
    | Escape
    | Selection of Selection
    | Deselection of Deselection
    | Move of newPointerPoint: Point
    | RequestAddItem of location: Point
    | AddBlock of addBlockPayload: AddBlockPayload
    | StartEditing of blockId: BlockId
    | BlockChange of BlockChange

[<Struct>]
type Connection = {
    Source: BlockId
    Target: BlockId
}

// type NextIds () =
//     let mutable nextBufferId = -1<_>
//     let mutable nextConstraintId = -1<_>
//
//     member _.NextBufferId =
//         nextBufferId <- nextBufferId + 1<_>
//         nextBufferId
//
//     member _.NextConstraintId =
//         nextConstraintId <- nextConstraintId
//         nextConstraintId


type NextIds () =
    let mutable nextBlockId: BlockId = -1<_>
    let mutable nextBufferId: BufferId = -1<_>
    let mutable nextConstraintId: ConstraintId = -1<_>
    let mutable nextConversionId: ConversionId = -1<_>
    let mutable nextConveyorId: ConveyorId = -1<_>
    let mutable nextMergeId: MergeId = -1<_>
    let mutable nextSplitId: SplitId = -1<_>

    member _.NextBlockId () =
        nextBlockId <- nextBlockId + 1<_>
        nextBlockId

    member _.NextBufferId () =
        nextBufferId <- nextBufferId + 1<_>
        nextBufferId

    member _.NextConstraintId () =
        nextConstraintId <- nextConstraintId + 1<_>
        nextConstraintId

    member _.NextConversionId() =
        nextConversionId <- nextConversionId + 1<_>
        nextConversionId

    member _.NextConveyorId () =
        nextConveyorId <- nextConveyorId + 1<_>
        nextConveyorId

    member _.NextMergeId () =
        nextMergeId <- nextMergeId + 1<_>
        nextMergeId

    member _.NextSplitId () =
        nextSplitId <- nextSplitId + 1<_>
        nextSplitId



type State =

    // member val NextIds = NextIds() with get
    // member val PointerState = PointerState.Neutral with get, set
    // member val Blocks = {
    //         Count = 0
    //         Names = Dictionary()
    //         Locations = Dictionary()
    //         Types = Dictionary()
    //         Attributes = Attributes.init ()
    //     } with get
    // member val Connections = HashSet<Connection>() with get
    // member val PointerLocation = Point (0.0, 0.0) with get, set
    // member val AddElementMenuLocation = Point (0.0, 0.0) with get, set
    // member val EditBlock : BlockId option = None with get, set

    {
        NextIds: NextIds
        PointerState: PointerState
        Blocks: Blocks
        PointerLocation: Point
        Connections: HashSet<Connection>
        AddElementMenuLocation: Point
        EditBlock: BlockId voption
    }

module State =

    let init () =
        {
            NextIds = NextIds()
            PointerState = PointerState.Neutral
            Blocks =
                {
                    Names = Dictionary()
                    Locations = Dictionary()
                    Types = Dictionary()
                    Attributes = Attributes.init ()
                }
            Connections = HashSet()
            PointerLocation = Point (0.0, 0.0)
            AddElementMenuLocation = Point (0.0, 0.0)
            EditBlock = ValueNone
        }

    let update (msg: Msg) (state: State) : State =
        let newState =
            match msg with
            | Msg.Escape ->
                { state with
                    PointerState = PointerState.Neutral
                    EditBlock = ValueNone }

            | Msg.StartEditing blockIdx ->
                { state with
                    EditBlock = ValueSome blockIdx }

            | Msg.BlockChange blockChange ->
                match blockChange with
                | BlockChange.Buffer bufferChange ->
                    match bufferChange with
                    | BufferChange.InitialVolume(bufferId, newInitialVolume) ->
                        state.Blocks.Attributes.Buffer[bufferId] <- { state.Blocks.Attributes.Buffer[bufferId] with InitialVolume = newInitialVolume }
                        state

                    | BufferChange.Capacity(bufferId, newCapacity) ->
                        state.Blocks.Attributes.Buffer[bufferId] <- { state.Blocks.Attributes.Buffer[bufferId] with Capacity =  newCapacity }
                        state

                | BlockChange.Constraint constraintChange ->
                    match constraintChange with
                    | ConstraintChange.Limit(constraintId, newLimit) ->
                        state.Blocks.Attributes.Constraints[constraintId] <- { state.Blocks.Attributes.Constraints[constraintId] with Limit = newLimit }
                        state

                | BlockChange.Conversion conversionChange ->
                    match conversionChange with
                    | ConversionChange.Coefficient(conversionId, newCoefficient) ->
                        state.Blocks.Attributes.Conversions[conversionId] <- { state.Blocks.Attributes.Conversions[conversionId] with Coefficient = newCoefficient }
                        state

                | BlockChange.Conveyor conveyorChange ->
                    match conveyorChange with
                    | ConveyorChange.Height(conveyorId, newHeight) ->
                        state.Blocks.Attributes.Conveyors[conveyorId] <- { state.Blocks.Attributes.Conveyors[conveyorId] with Height = newHeight }
                        state

                    | ConveyorChange.Length(conveyorId, newLength) ->
                        state.Blocks.Attributes.Conveyors[conveyorId] <- { state.Blocks.Attributes.Conveyors[conveyorId] with Length = newLength }
                        state

                    | ConveyorChange.MaxVelocity(conveyorId, newMaxVelocity) ->
                        state.Blocks.Attributes.Conveyors[conveyorId] <- { state.Blocks.Attributes.Conveyors[conveyorId] with MaxVelocity =  newMaxVelocity }
                        state

            | Msg.AddBlock addBlockPayload ->
                let blocks = state.Blocks
                let newBlockId = state.NextIds.NextBlockId()

                match addBlockPayload.AddBlockType with
                | AddBlockType.Buffer ->
                    let newBufferId = state.NextIds.NextBufferId()
                    let newName = $"Buffer {newBufferId}"
                    blocks.Names[newBlockId] <- newName
                    blocks.Types[newBlockId] <- BlockType.Buffer newBufferId
                    blocks.Locations[newBlockId] <- addBlockPayload.Location
                    blocks.Attributes.Buffer[newBufferId] <- { InitialVolume = 100.0; Capacity = 100.0 }

                | AddBlockType.Constraint ->
                    let newConstraintId = state.NextIds.NextConstraintId()
                    let newName = $"Constraint {newConstraintId}"
                    blocks.Names[newBlockId] <- newName
                    blocks.Types[newBlockId] <- BlockType.Constraint newConstraintId
                    blocks.Locations[newBlockId] <- addBlockPayload.Location
                    blocks.Attributes.Constraints[newConstraintId] <- { Limit = 1.0 }

                | AddBlockType.Conversion ->
                    let newConversionId = state.NextIds.NextConversionId()
                    let newName = $"Conversion {newConversionId}"
                    blocks.Names[newBlockId] <- newName
                    blocks.Types[newBlockId] <- BlockType.Conversion newConversionId
                    blocks.Locations[newBlockId] <- addBlockPayload.Location
                    blocks.Attributes.Conversions[newConversionId] <- { Coefficient =  1.0 }

                | AddBlockType.Conveyor ->
                    let newConveyorId = state.NextIds.NextConveyorId()
                    let newName = $"Conveyor {newConveyorId}"
                    blocks.Names[newBlockId] <- newName
                    blocks.Types[newBlockId] <- BlockType.Conveyor newConveyorId
                    blocks.Locations[newBlockId] <- addBlockPayload.Location
                    blocks.Attributes.Conveyors[newConveyorId] <- { Height = 1.0; Length = 1.0; MaxVelocity = 1.0 }

                | AddBlockType.Merge ->
                    let newMergeId = state.NextIds.NextMergeId()
                    let newName = $"Merge {newMergeId}"
                    blocks.Names[newBlockId] <- newName
                    blocks.Types[newBlockId] <- BlockType.Merge newMergeId
                    blocks.Locations[newBlockId] <- addBlockPayload.Location
                    blocks.Attributes.Merges[newMergeId] <- { Type = Attributes.MergeType.Priority Array.empty }

                | AddBlockType.Split ->
                    let newSplitId = state.NextIds.NextSplitId()
                    let newName = $"Split {newSplitId}"
                    blocks.Names[newBlockId] <- newName
                    blocks.Types[newBlockId] <- BlockType.Split newSplitId
                    blocks.Locations[newBlockId] <- addBlockPayload.Location
                    blocks.Attributes.Splits[newSplitId] <- { Type = Attributes.SplitType.Priority Array.empty }

                { state with PointerState = PointerState.Neutral }

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
                    | Selection.Output blockIdx ->
                        { state with
                            PointerState = PointerState.ConnectingOutput blockIdx }
                    | Selection.Input blockIdx ->
                        { state with
                            PointerState = PointerState.ConnectingInput blockIdx }

                | PointerState.ConnectingOutput sourceBlockId ->
                    match selection with
                    | Selection.Input blockId ->
                        if sourceBlockId <> blockId then
                            state.Connections.Add { Source = sourceBlockId; Target = blockId }
                            |> ignore
                            { state with
                                PointerState = PointerState.Neutral }
                        else
                            state

                    | _ ->
                        state

                | PointerState.ConnectingInput inputBlockId ->
                    match selection with
                    | Selection.Output blockId ->
                        if inputBlockId <> blockId then
                            state.Connections.Add { Source = blockId; Target = inputBlockId }
                            |> ignore
                            { state with
                                PointerState = PointerState.Neutral }
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

        newState
