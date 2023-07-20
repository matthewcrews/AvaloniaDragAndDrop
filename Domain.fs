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
    | Panning


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

type RemoveBlockPayload = {
    BlockId: BlockId
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

type BlockChangePayload =
    | Buffer of BufferChange
    | Constraint of ConstraintChange
    | Conversion of ConversionChange
    | Conveyor of ConveyorChange

type MoveBlockPayload = {
    BlockId: BlockId
    NewLocation: Point
}

type AddConnectionPayload = {
    Source: BlockId
    Target: BlockId
}

type RemoveConnectionPayload = {
    Source: BlockId
    Target: BlockId
}

[<RequireQualifiedAccess>]
type Cmd =
    | AddBlock of addBlockPayload: AddBlockPayload
    | RemoveBlock of removeBlockPayload: RemoveBlockPayload
    | AddConnection of addConnectionPayload: AddConnectionPayload
    | RemoveConnection of removeConnectionPayload: RemoveConnectionPayload
    | ChangeBlock of BlockChangePayload
    | MoveBlock of moveBlockPayload: MoveBlockPayload


[<RequireQualifiedAccess>]
type Msg =
    | Cmd of cmd: Cmd
    | Escape
    | Selection of Selection
    | Deselection of Deselection
    | Move of newPointerPoint: Point
    | RequestAddItem of location: Point
    | StartEditing of blockId: BlockId
    | PanningStarted
    | PanningStopped
    | ZoomIn
    | ZoomOut
    | SaveRequested
    | OpenLoadScreen
    | SaveFileNameChanged of string
    | LoadFile of fileName: string

[<Struct>]
type Connection = {
    Source: BlockId
    Target: BlockId
}

type NextIds =
    {
        mutable BlockId: BlockId
        mutable BufferId: BufferId
        mutable ConstraintId: ConstraintId
        mutable ConversionId: ConversionId
        mutable ConveyorId: ConveyorId
        mutable MergeId: MergeId
        mutable SplitId: SplitId
    }
    static member init () =
        {
            BlockId = -1<_>
            BufferId = -1<_>
            ConstraintId = -1<_>
            ConversionId = -1<_>
            ConveyorId = -1<_>
            MergeId = -1<_>
            SplitId = -1<_>
        }

    member n.NextBlockId () =
        n.BlockId <- n.BlockId + 1<_>
        n.BlockId

    member n.NextBufferId () =
        n.BufferId <- n.BufferId + 1<_>
        n.BufferId

    member n.NextConstraintId () =
        n.ConstraintId <- n.ConstraintId + 1<_>
        n.ConstraintId

    member n.NextConversionId() =
        n.ConversionId <- n.ConversionId + 1<_>
        n.ConversionId

    member n.NextConveyorId () =
        n.ConveyorId <- n.ConveyorId + 1<_>
        n.ConveyorId

    member n.NextMergeId () =
        n.MergeId <- n.MergeId + 1<_>
        n.MergeId

    member n.NextSplitId () =
        n.SplitId <- n.SplitId + 1<_>
        n.SplitId


[<RequireQualifiedAccess>]
type Window =
    | Editor
    | Save
    | Load

type State =
    {
        Window: Window
        FileName: string option
        OutputDirectory: string option
        WindowPosition: Point
        Zoom: float
        mutable NextIds: NextIds
        PointerState: PointerState
        Blocks: Blocks
        PointerPosition: Point
        Connections: Set<Connection>
        AddElementMenuLocation: Point
        EditBlock: BlockId voption
        History: list<Cmd>
    }

module State =

    let init () =
        {
            Window = Window.Editor
            FileName = None
            OutputDirectory = None
            WindowPosition = Point(0.0, 0.0)
            Zoom = 1.0
            NextIds = NextIds.init ()
            PointerState = PointerState.Neutral
            Blocks =
                {
                    Names = Dictionary()
                    Locations = Dictionary()
                    Types = Dictionary()
                    Attributes = Attributes.init ()
                }
            Connections = Set.empty
            PointerPosition = Point (0.0, 0.0)
            AddElementMenuLocation = Point (0.0, 0.0)
            EditBlock = ValueNone
            History = []
        }

    module private Cmd =


        let handle (state: State) (cmd: Cmd) : State =
            let newState =
                match cmd with
                | Cmd.AddBlock addBlockPayload ->
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

                | Cmd.RemoveBlock removeBlockPayload ->
                    notImplementedExn()

                | Cmd.AddConnection addConnectionPayload ->
                    let newConnection : Connection = {
                        Source = addConnectionPayload.Source
                        Target = addConnectionPayload.Target
                    }
                    { state with
                        Connections = state.Connections.Add newConnection
                        PointerState = PointerState.Neutral }


                | Cmd.RemoveConnection removeConnectionPayload ->
                    notImplementedExn ()

                | Cmd.ChangeBlock blockChangePayload ->

                    match blockChangePayload with
                    | BlockChangePayload.Buffer bufferChange ->
                        match bufferChange with
                        | BufferChange.InitialVolume(bufferId, newInitialVolume) ->
                            state.Blocks.Attributes.Buffer[bufferId] <- { state.Blocks.Attributes.Buffer[bufferId] with InitialVolume = newInitialVolume }
                            state

                        | BufferChange.Capacity(bufferId, newCapacity) ->
                            state.Blocks.Attributes.Buffer[bufferId] <- { state.Blocks.Attributes.Buffer[bufferId] with Capacity =  newCapacity }
                            state

                    | BlockChangePayload.Constraint constraintChange ->
                        match constraintChange with
                        | ConstraintChange.Limit(constraintId, newLimit) ->
                            state.Blocks.Attributes.Constraints[constraintId] <- { state.Blocks.Attributes.Constraints[constraintId] with Limit = newLimit }
                            state

                    | BlockChangePayload.Conversion conversionChange ->
                        match conversionChange with
                        | ConversionChange.Coefficient(conversionId, newCoefficient) ->
                            state.Blocks.Attributes.Conversions[conversionId] <- { state.Blocks.Attributes.Conversions[conversionId] with Coefficient = newCoefficient }
                            state

                    | BlockChangePayload.Conveyor conveyorChange ->
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

                | Cmd.MoveBlock moveBlockPayload ->
                    state.Blocks.Locations[moveBlockPayload.BlockId] <- moveBlockPayload.NewLocation
                    { state with
                        PointerState = PointerState.Neutral }

            { newState with
                History = cmd :: newState.History }


    let rec update (msg: Msg) (state: State) : State =
        let newState =
            match msg with
            | Msg.Cmd cmd ->
                Cmd.handle state cmd

            | Msg.Escape ->
                { state with
                    PointerState = PointerState.Neutral
                    EditBlock = ValueNone }

            | Msg.SaveFileNameChanged newFileName ->
                { state with
                    FileName = Some newFileName }

            | Msg.SaveRequested ->

                match state.FileName with
                | None ->
                    { state with Window = Window.Save }

                | Some fileName ->
                    let outputDirectory =
                        state.OutputDirectory
                        |> Option.defaultValue (System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile))
                    let fullFilename = $"{fileName}.aidos"
                    let savePath = System.IO.Path.Combine (outputDirectory, fullFilename)
                    let saveData = Newtonsoft.Json.JsonConvert.SerializeObject state
                    System.IO.File.WriteAllText (savePath, saveData)
                    { state with
                        Window = Window.Editor }

            | Msg.OpenLoadScreen ->
                { state with
                    Window = Window.Load }

            | Msg.LoadFile fileName ->
                let outputDirectory =
                    state.OutputDirectory
                    |> Option.defaultValue (System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile))
                let filePath = System.IO.Path.Combine (outputDirectory, fileName)
                let fileData = System.IO.File.ReadAllText filePath
                let newState = Newtonsoft.Json.JsonConvert.DeserializeObject<State> fileData
                { newState with
                    Window = Window.Editor }

            | Msg.PanningStarted ->
                { state with
                    PointerState = PointerState.Panning }

            | Msg.PanningStopped ->
                { state with
                    PointerState = PointerState.Neutral  }

            | Msg.ZoomIn ->
                let newZoom = System.Math.Clamp (state.Zoom * 1.2, 0.5, 10.0)
                let windowLocationDelta = state.WindowPosition - state.PointerPosition
                let newWindowLocationPointerOffset = (newZoom / state.Zoom) * windowLocationDelta
                let newWindowPosition = state.PointerPosition + newWindowLocationPointerOffset
                { state with
                    Zoom = newZoom
                    WindowPosition = newWindowPosition }

            | Msg.ZoomOut ->
                let newZoom = System.Math.Clamp (state.Zoom * 0.8, 0.5, 10.0)
                let windowLocationDelta = state.WindowPosition - state.PointerPosition
                let newWindowLocationPointerOffset = (newZoom / state.Zoom) * windowLocationDelta
                let newWindowPosition = state.PointerPosition + newWindowLocationPointerOffset
                { state with
                    Zoom = newZoom
                    WindowPosition = newWindowPosition }

            | Msg.StartEditing blockIdx ->
                { state with
                    EditBlock = ValueSome blockIdx }




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
                            PointerPosition = position }
                    | Selection.Output blockIdx ->
                        { state with
                            PointerState = PointerState.ConnectingOutput blockIdx }
                    | Selection.Input blockIdx ->
                        { state with
                            PointerState = PointerState.ConnectingInput blockIdx }

                | PointerState.ConnectingOutput sourceBlockId ->
                    match selection with
                    | Selection.Input targetBlockId ->
                        if sourceBlockId <> targetBlockId then
                            let newConnectionPayload : AddConnectionPayload = {
                                Source = sourceBlockId
                                Target = targetBlockId
                            }
                            let msg = Msg.Cmd (Cmd.AddConnection newConnectionPayload)
                            update msg state
                        else
                            state

                    | _ ->
                        state

                | PointerState.ConnectingInput targetBlockId ->
                    match selection with
                    | Selection.Output sourceBlockId ->
                        if targetBlockId <> sourceBlockId then
                            let newConnectionPayload : AddConnectionPayload = {
                                Source = sourceBlockId
                                Target = targetBlockId
                            }
                            let msg = Msg.Cmd (Cmd.AddConnection newConnectionPayload)
                            update msg state

                        else
                            state
                    | _ ->
                        state
                | _ ->
                    state

            | Msg.Deselection deselection ->

                match state.PointerState, deselection with
                | PointerState.Dragging blockId, _ ->
                    let newLocation = state.Blocks.Locations[blockId]
                    let moveBlockPayload : MoveBlockPayload = {
                        BlockId = blockId
                        NewLocation = newLocation
                    }
                    let msg = Msg.Cmd (Cmd.MoveBlock moveBlockPayload)
                    update msg state

                    // { state with
                    //     PointerState = PointerState.Neutral }

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
                    let delta = newPointerPoint - state.PointerPosition
                    let newState =
                        { state with
                            PointerPosition = newPointerPoint }
                    newState.Blocks.Locations[blockIdx] <- state.Blocks.Locations[blockIdx] + delta / state.Zoom
                    newState

                | PointerState.Panning ->
                    let delta = newPointerPoint - state.PointerPosition
                    { state with
                        PointerPosition = newPointerPoint
                        WindowPosition = state.WindowPosition + delta }

                | _ ->
                    { state with
                        PointerPosition = newPointerPoint }

        newState
