using System;

namespace ERBingoRandomizer.Utility;

class InvalidParamDefException : Exception {
    public InvalidParamDefException(string paramType) : base($"Could not find ParamDef for {paramType}") { }

    public InvalidParamDefException(string paramType, Exception inner) : base($"Could not find ParamDef for {paramType}", inner) { }
}

public class InvalidFileException : Exception {
    public InvalidFileException(string filePath) : base($"Could not find file {filePath}") { }

    public InvalidFileException(string filePath, Exception inner) : base($"Could not find file {filePath}", inner) { }
}

public class BinderFileNotFoundException : Exception {
    public BinderFileNotFoundException(string filePath) : base($"Could not find binder file file {filePath}") { }

    public BinderFileNotFoundException(string filePath, Exception inner) : base($"Could not find binder file file {filePath}", inner) { }
}
