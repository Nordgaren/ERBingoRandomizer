using System;

namespace ERBingoRandomizer; 
class NoParamDefException : Exception {
    public NoParamDefException(string paramType) : base($"Could not find ParamDef for {paramType}") {}

    public NoParamDefException(string paramType, Exception inner) : base($"Could not find ParamDef for {paramType}", inner) {}
}

public class InvalidFileException : Exception {
    public InvalidFileException(string filePath) : base($"Could not find file {filePath}") {}

    public InvalidFileException(string filePath, Exception inner) : base($"Could not find file {filePath}", inner) {}
}

