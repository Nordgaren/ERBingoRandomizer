#pragma once
#include "modengine/util/memory_scanner.h"

namespace modengine {
    /**
     * Storing a list of function pointers as void* is undefined behaviour in C++, however, casting between
     * function pointers of different types is completely legal.
     */
    using GenericFunctionPointer = void (*)();

    template <typename T>
    struct Hook {
        Hook()
            : applied(false)
              , original(nullptr)
              , replacement(nullptr) {
        }

        Hook(uintptr_t _original, T _replacement)
            : applied(false)
              , original(static_cast<T>(_original))
              , replacement(_replacement) {
        }

        Hook(T _original, T _replacement)
            : applied(false)
              , original(_original)
              , replacement(_replacement) {
        }

        ~Hook() {
            spdlog::info("Destroying");
        }

        bool applied;
        T original;
        T replacement;
    };

    using HookScanMode = enum {
        SCAN_FUNCTION,
        // The scanned result is the beginning of the target function
        SCAN_CALL_INST,
        // The scanned result is a call instruction to the target function
    };

    template <typename T>
    struct ScannedHook {
        ScannedHook()
            : applied(false)
              , mode(SCAN_FUNCTION)
              , original(nullptr)
              , replacement(nullptr) {
        }

        ScannedHook(HookScanMode _mode, ScanPattern _pattern, T _replacement)
            : applied(false)
              , mode(_mode)
              , pattern(_pattern)
              , original(nullptr)
              , replacement(_replacement) {
        }

        ~ScannedHook() {
            spdlog::info("Destroying");
        }

        bool applied;
        HookScanMode mode;
        ScanPattern pattern;
        T original;
        T replacement;
    };
}
