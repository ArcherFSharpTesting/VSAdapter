module Archer.Quiver.TestAdapter.Source

open System.IO

let getDirectory _source = Directory.GetCurrentDirectory () |> DirectoryInfo