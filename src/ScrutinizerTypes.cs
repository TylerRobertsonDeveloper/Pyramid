﻿using System.Collections.Generic;

namespace Pyramid.Scrutinizer
{
    public enum BranchCategory
    {
        FORK_BRANCH,        /// Branch for which both targets are nested in same loop as current block
        CONTINUE_BRANCH,    /// Fork branch where one target is loop's header
        SKIP_BRANCH,        /// Fork branch where one target is the post-dominator of the loop
        LOOPSKIP_BRANCH,    /// A skip branch which jumps around a complete loop

        BREAK_BRANCH,       /// Not a fork branch
         
    };

    public class BasicBlock
    {
        public IEnumerable<IInstruction> Instructions { get { return m_Instructions; } }
        public IEnumerable<BasicBlock> Predecessors { get { return m_Predecessors; } }
        public IEnumerable<BasicBlock> Successors { get { return m_Successors; } }
        public int SuccessorCount { get { return m_Successors.Count;  } }
        public IInstruction LastInstruction { get { return m_Instructions[m_Instructions.Count - 1]; } }

        public BasicBlock PostDominator { get; set; }
        public BasicBlock ImmediateDominator { get; set; }
        public Loop InnerMostLoop { get; set; }
        public void AddSuccessor(BasicBlock bl) { m_Successors.Add(bl); }
        public void AddPredecessor(BasicBlock bl) { m_Predecessors.Add(bl); }
        public void AddInstruction(IInstruction i) { m_Instructions.Add(i); }
        private List<IInstruction> m_Instructions = new List<IInstruction>();
        private List<BasicBlock> m_Predecessors = new List<BasicBlock>();
        private List<BasicBlock> m_Successors = new List<BasicBlock>();

        public bool Dominates( BasicBlock b )
        {
            if (b == this)
                return true;

            BasicBlock dom = b.ImmediateDominator;
            while( dom != null )
            {
                if (dom == this)
                    return true;
                dom = dom.ImmediateDominator;
            }
            return false;
        }
    };

    public class Loop
    {
        private List<BasicBlock> m_Blocks;
        private BasicBlock m_Header;
        
        public Loop( BasicBlock header, IEnumerable<BasicBlock> blocks )
        {
            m_Header = header;
            m_Blocks = new List<BasicBlock>(blocks);
            Parent = null;
            DesiredIterations = 1;
        }

        public int DesiredIterations { get; set; }

        public IEnumerable<BasicBlock> Blocks { get { return m_Blocks; } }
        public BasicBlock Header { get { return m_Header; } }
        public Loop Parent { get; set; }
        public int BlockCount { get { return m_Blocks.Count; } }

        public bool IsNestedIn( Loop p )
        {
            Loop n = this;
            while (n != null)
            {
                if (n == p)
                    return true;
                n = n.Parent;
            }
            return false;
        }
    }

    public interface IInstruction
    {
        BasicBlock Block { get; set;  }
        string Label { get; set; }
        string SimNotes { get; set; } // String field used to display simulation results
        string Disassemble();
    };

    public interface IBranchInstruction : IInstruction
    {
        BranchCategory Category { get; set; }
        IInstruction IfTarget { get; }
        IInstruction ElseTarget { get; }
    };

    public interface IJumpInstruction : IInstruction
    {
        IInstruction Target { get; }
    };

    public enum TexelFormat
    {
        R8,
        RG8,
        RGBA8,
        R16,
        RG16,
        RGBA16,
        R16F,
        RG16F,
        RGBA16F,
        R32F,
        RG32F,
        RGBA32F,
        BC1,
        BC2,
        BC3,
        BC4,
        BC5,
        BC6,
        BC7
    };
    public enum TextureFilter
    {
        POINT,
        BILINEAR,
        TRILINEAR,
        ANISO_2X,
        ANISO_4X,
        ANISO_8X
    };

    /// <summary>
    ///  Interface for instructions which can be annoted with a texture format
    /// </summary>
    public interface ITextureInstruction : IInstruction
    {
        TexelFormat Format { get; set; }
    };

    /// <summary>
    ///  Interface for instructions which can be annoted with a texture format
    ///   or a filtering mode
    /// </summary>
    public interface ISamplingInstruction : ITextureInstruction
    {
        TextureFilter Filter { get; set; }
    };

   

    public interface ISimulationParameter
    {
        string Name { get; }
    };

    public class SimulationParameterInt : ISimulationParameter
    {
        public SimulationParameterInt( int nMin, int nMax, int nDefault, string sName )
        {
            Min = nMin;
            Max = nMax;
            Default = nDefault;
            Value = Default;
            Name = sName;
        }
        public string Name { get; private set; }
        public int Min     { get; private set; }
        public int Max     { get; private set; }
        public int Default { get; private set; }
        public int Value   { get; set; }
    };

    public class SimulationParameterDouble : ISimulationParameter
    {
        public SimulationParameterDouble(double fMin, double fMax, double fDefault, string sName )
        {
            Min     = fMin;
            Max     = fMax;
            Default = fDefault;
            Value   = Default;
            Name    = sName;
        }
        public string Name { get; private set;  }
        public double Min     { get; private set; }
        public double Max     { get; private set; }
        public double Default { get; private set; }
        public double Value   { get; set; }
    };



    public interface IScrutinizer
    {
        List<IInstruction> BuildProgram( );

        List<IInstruction> BuildDXFetchShader(IDXShaderReflection refl);

        IEnumerable<ISimulationParameter> SimulationParameters { get; }

        string AnalyzeExecutionTrace(List<IInstruction> ops);

    };

}