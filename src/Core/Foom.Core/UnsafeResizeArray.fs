    namespace Foom.Core

    open System
    
    [<ReferenceEquality>]
    type UnsafeResizeArray<'T> =
        {
            mutable count: int
            mutable buffer: 'T []
        }

        static member Create capacity =
            if capacity <= 0 then
                failwith "Capacity must be greater than 0"

            {
                count = 0
                buffer = Array.zeroCreate<'T> capacity
            }

        //static member Create (buffer: 'T []) =
        //    {
        //        count = buffer.Length
        //        buffer = buffer
        //    }

        member this.IncreaseCapacity () =
            let newLength = uint32 this.buffer.Length * 2u
            if newLength >= uint32 Int32.MaxValue then
                failwith "Length is bigger than the maximum number of elements in the array"

            let newBuffer = Array.zeroCreate<'T> (int newLength)
            Array.Copy (this.buffer, newBuffer, this.count)
            this.buffer <- newBuffer
             

        member inline this.Add item =
            if this.count >= this.buffer.Length then
                this.IncreaseCapacity ()
            
            this.buffer.[this.count] <- item
            this.count <- this.count + 1

        member inline this.LastItem = this.buffer.[this.count - 1]

        member inline this.SwapRemoveAt index =
            if index >= this.count then
                failwith "Index out of bounds"

            let lastIndex = this.count - 1

            this.buffer.[index] <- this.buffer.[lastIndex]
            this.buffer.[lastIndex] <- Unchecked.defaultof<'T>
            this.count <- lastIndex

        member inline this.Count = this.count
        member inline this.Buffer = this.buffer