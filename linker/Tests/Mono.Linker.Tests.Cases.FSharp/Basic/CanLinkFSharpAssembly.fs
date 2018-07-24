namespace Mono.Linker.Tests.Cases.FSharp.Basic

module CanLinkFSharpAssembly =
#if ENTRY
    [<EntryPoint>]
#endif
    let main argv = 
        printfn "%A" argv
        0 // return an integer exit code