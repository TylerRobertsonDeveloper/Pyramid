//
//    GCN disassembler
//      Copyright 2015 Joshua Barczak, all rights reserved.
//
#ifndef _GCN_DISASSEMBLER_H_
#define _GCN_DISASSEMBLER_H_
#pragma once

namespace GCN{
    class Instruction;
    class IDecoder;
namespace Disassembler{

    /// Interface that receives a series of text strings
    class IPrinter
    {
    public:
        virtual void Push( const char* pText ) = 0;
    };

    /// Disassemble a single instruction
    ///   Instruction is assumed to be well formed
    ///    If pLabelName is non-null it will be used as the name of the target instruction
    ///     for a branch/jump.
    ///
    void Disassemble( IPrinter& printer, const Instruction* pIt, const char* pLabelName );

    /// Disassemble a full program, returns false if an instruction is encountered whose encoding
    ///  wasn't recognized
    bool DisassembleProgram( IDecoder& decoder, IPrinter& printer, const void* pISA, size_t nISASize );

    /// List encodings and hex bytes for each instruction.  Returns false if an instruction is encountered
    ///  whose encoding wasn't recognized. In this case, returns early
    bool ListEncodings( IDecoder& decoder, IPrinter& printer, const void* pISA, size_t nISASize );
}}

#endif