namespace MathSymbols;

internal sealed record MathSymbol(string Glyph, string Name, string SearchTerms, string Category);

internal static class SymbolCatalog
{
    // Search terms include the common names and phrases people use in class and in textbooks.
    private static readonly List<MathSymbol> Items = Build();
    public static IReadOnlyList<MathSymbol> All => Items;

    private static List<MathSymbol> Build()
    {
        var items = new List<MathSymbol>();
        void Add(string glyph, string name, string terms, string category) => items.Add(new(glyph, name, terms, category));

        // Calculus, analysis, and common operators
        Add("∫", "Integral", "integration antiderivative area under curve", "Calculus");
        Add("∬", "Double integral", "double integration area surface", "Calculus");
        Add("∭", "Triple integral", "triple integration volume", "Calculus");
        Add("∮", "Contour integral", "line integral closed curve complex", "Calculus");
        Add("∯", "Surface integral", "double contour integral", "Calculus");
        Add("∰", "Volume integral", "triple contour integral", "Calculus");
        Add("∱", "Clockwise integral", "clockwise contour integral", "Calculus");
        Add("⨌", "Quadruple integral", "fourfold integration", "Calculus");
        Add("∂", "Partial derivative", "partial differential d", "Calculus");
        Add("∇", "Nabla", "del gradient divergence curl", "Calculus");
        Add("∆", "Increment", "delta change difference laplacian", "Calculus");
        Add("∑", "Summation", "sum sigma series", "Calculus");
        Add("∏", "Product", "product pi multiplication series", "Calculus");
        Add("∐", "Coproduct", "coproduct disjoint union", "Calculus");
        Add("∞", "Infinity", "infinite unbounded", "Calculus");
        Add("√", "Square root", "radical root", "Calculus");
        Add("∛", "Cube root", "third root radical", "Calculus");
        Add("∜", "Fourth root", "fourth root radical", "Calculus");
        Add("ℓ", "Script small L", "ell length line", "Calculus");
        Add("ℏ", "Reduced Planck constant", "h bar quantum", "Calculus");
        Add("′", "Prime", "derivative minutes feet", "Calculus");
        Add("″", "Double prime", "second derivative seconds inches", "Calculus");
        Add("‴", "Triple prime", "third derivative", "Calculus");
        Add("⁗", "Quadruple prime", "fourth derivative", "Calculus");
        Add("°", "Degree", "degrees angle temperature", "Geometry");
        Add("‰", "Per mille", "permille per thousand", "Other");
        Add("‱", "Per ten thousand", "basis point permyriad", "Other");
        Add("⁄", "Fraction slash", "fraction divided by", "Algebra");
        Add("∕", "Division slash", "slash division", "Algebra");

        // Arithmetic, algebra, and number sets
        Add("+", "Plus", "addition positive", "Algebra"); Add("−", "Minus", "subtraction negative", "Algebra");
        Add("±", "Plus or minus", "plus-minus positive negative", "Algebra"); Add("∓", "Minus or plus", "minus-plus", "Algebra");
        Add("×", "Multiplication", "times multiply cross product", "Algebra"); Add("÷", "Division", "divide obelus", "Algebra");
        Add("·", "Middle dot", "dot multiplication scalar product", "Algebra"); Add("⋅", "Dot operator", "dot product scalar multiplication", "Algebra");
        Add("∙", "Bullet operator", "bullet dot multiplication", "Algebra"); Add("∗", "Asterisk operator", "star convolution", "Algebra");
        Add("∘", "Composition", "function composition circle operator", "Algebra"); Add("⨯", "Vector cross product", "cross product vector", "Algebra");
        Add("⊙", "Circle dot", "circled dot", "Algebra"); Add("⊗", "Tensor product", "circled times tensor", "Algebra");
        Add("⊕", "Direct sum", "circled plus xor", "Algebra"); Add("⊖", "Circled minus", "circled subtraction", "Algebra");
        Add("⊘", "Circled division slash", "circled divide", "Algebra"); Add("⊛", "Circled asterisk", "circled star", "Algebra");
        Add("⊚", "Circled ring", "circle operator", "Algebra"); Add("⊞", "Squared plus", "square plus", "Algebra");
        Add("⊟", "Squared minus", "square minus", "Algebra"); Add("⊠", "Squared times", "square multiplication", "Algebra");
        Add("⊡", "Squared dot", "square dot", "Algebra"); Add("∔", "Dot plus", "plus dot", "Algebra");
        Add("∸", "Dot minus", "minus dot", "Algebra"); Add("∹", "Excess", "excess equals", "Algebra");
        Add("∺", "Geometric proportion", "geometric ratio", "Algebra"); Add("∻", "Homothetic", "homothetic", "Algebra");
        Add("∼", "Similar to", "similar similarity tilde", "Relations"); Add("≃", "Asymptotically equal", "asymptotic equivalence", "Relations");
        Add("≅", "Congruent to", "congruent approximately equal", "Geometry"); Add("≈", "Approximately equal", "approx equal about", "Relations");
        Add("≐", "Approaches the limit", "lim approaches", "Calculus"); Add("≑", "Geometrically equal", "geometrically equal", "Geometry");
        Add("≒", "Approximately equal or image", "roughly equal", "Relations"); Add("≓", "Image or approximately equal", "roughly equal", "Relations");
        Add("≔", "Defined as", "definition assignment equals", "Algebra"); Add("≕", "Equals by definition", "defined equal", "Algebra");
        Add("≜", "Delta equal to", "equal by definition", "Algebra"); Add("≝", "Equal by definition", "defined as equal", "Algebra");
        Add("≟", "Questioned equal to", "question equal", "Relations"); Add("≠", "Not equal", "unequal inequality", "Relations");
        Add("≡", "Equivalent to", "identical congruent equivalence", "Relations"); Add("≢", "Not identical to", "not equivalent", "Relations");
        Add("<", "Less than", "smaller inequality", "Relations"); Add(">", "Greater than", "larger inequality", "Relations");
        Add("≤", "Less than or equal", "less than equal to", "Relations"); Add("≥", "Greater than or equal", "greater than equal to", "Relations");
        Add("≪", "Much less than", "much smaller", "Relations"); Add("≫", "Much greater than", "much larger", "Relations");
        Add("≦", "Less than over equal", "less than or equal", "Relations"); Add("≧", "Greater than over equal", "greater than or equal", "Relations");
        Add("≮", "Not less than", "not smaller", "Relations"); Add("≯", "Not greater than", "not larger", "Relations");
        Add("≰", "Neither less than nor equal", "not less than equal", "Relations"); Add("≱", "Neither greater than nor equal", "not greater than equal", "Relations");
        Add("≲", "Less than or equivalent", "less than similar", "Relations"); Add("≳", "Greater than or equivalent", "greater than similar", "Relations");
        Add("≶", "Less than or greater than", "less or greater", "Relations"); Add("≷", "Greater than or less than", "greater or less", "Relations");
        Add("≺", "Precedes", "precedes order", "Relations"); Add("≻", "Succeeds", "succeeds order", "Relations");
        Add("≼", "Precedes or equal", "precedes equal", "Relations"); Add("≽", "Succeeds or equal", "succeeds equal", "Relations");
        Add("∝", "Proportional to", "proportion varies as", "Algebra"); Add("∣", "Divides", "divisibility divides", "Algebra");
        Add("∤", "Does not divide", "not divisible", "Algebra"); Add("∥", "Parallel to", "parallel", "Geometry");
        Add("∦", "Not parallel to", "nonparallel", "Geometry"); Add("∠", "Angle", "angle geometry", "Geometry");
        Add("∡", "Measured angle", "directed angle", "Geometry"); Add("∢", "Spherical angle", "angle geometry", "Geometry");

        // Set theory, logic, and proof notation
        Add("∈", "Element of", "belongs to member of in set", "Sets & Logic"); Add("∉", "Not an element of", "not member set", "Sets & Logic");
        Add("∋", "Contains as member", "has element contains", "Sets & Logic"); Add("∌", "Does not contain as member", "not contains element", "Sets & Logic");
        Add("∅", "Empty set", "null set phi", "Sets & Logic"); Add("∪", "Union", "set union cup", "Sets & Logic");
        Add("∩", "Intersection", "set intersection cap", "Sets & Logic"); Add("∖", "Set minus", "set difference backslash", "Sets & Logic");
        Add("∧", "Logical AND", "and wedge conjunction", "Sets & Logic"); Add("∨", "Logical OR", "or vee disjunction", "Sets & Logic");
        Add("¬", "Logical NOT", "not negation", "Sets & Logic"); Add("⊻", "Exclusive OR", "xor exclusive or", "Sets & Logic");
        Add("⊼", "NAND", "not and", "Sets & Logic"); Add("⊽", "NOR", "not or", "Sets & Logic");
        Add("∀", "For all", "universal quantifier every", "Sets & Logic"); Add("∃", "There exists", "existential quantifier", "Sets & Logic");
        Add("∄", "There does not exist", "not exists", "Sets & Logic"); Add("∁", "Complement", "set complement", "Sets & Logic");
        Add("⊂", "Subset of", "proper subset contained in", "Sets & Logic"); Add("⊃", "Superset of", "proper superset contains", "Sets & Logic");
        Add("⊆", "Subset or equal", "subset equals", "Sets & Logic"); Add("⊇", "Superset or equal", "superset equals", "Sets & Logic");
        Add("⊄", "Not a subset", "not subset", "Sets & Logic"); Add("⊅", "Not a superset", "not superset", "Sets & Logic");
        Add("⊈", "Neither subset nor equal", "not subset equal", "Sets & Logic"); Add("⊉", "Neither superset nor equal", "not superset equal", "Sets & Logic");
        Add("⊊", "Subset with not equal", "proper subset unequal", "Sets & Logic"); Add("⊋", "Superset with not equal", "proper superset unequal", "Sets & Logic");
        Add("⊢", "Proves", "turnstile entails", "Sets & Logic"); Add("⊣", "Reverse turnstile", "left tack", "Sets & Logic");
        Add("⊨", "Models", "models satisfies semantic entailment", "Sets & Logic"); Add("⊩", "Forces", "forces turnstile", "Sets & Logic");
        Add("⊪", "Triple vertical bar right turnstile", "proves", "Sets & Logic"); Add("⊬", "Does not prove", "not turnstile", "Sets & Logic");
        Add("⊭", "Does not model", "not models", "Sets & Logic"); Add("⊮", "Does not force", "not forces", "Sets & Logic");
        Add("∴", "Therefore", "hence thus", "Sets & Logic"); Add("∵", "Because", "since", "Sets & Logic");
        Add("⊤", "Top", "true tautology", "Sets & Logic"); Add("⊥", "Bottom", "false contradiction perpendicular", "Sets & Logic");
        Add("⊦", "Assertion", "assertion turnstile", "Sets & Logic"); Add("⊧", "Models", "double turnstile", "Sets & Logic");
        Add("∎", "End of proof", "qed tombstone halmos", "Sets & Logic"); Add("□", "Square", "white square qed", "Geometry");
        Add("◇", "Diamond", "lozenge modal possible", "Sets & Logic"); Add("◆", "Black diamond", "filled diamond", "Geometry");

        // Geometry and geometric shapes
        Add("⊾", "Right angle with arc", "angle", "Geometry"); Add("⊿", "Right triangle", "triangle geometry", "Geometry");
        Add("△", "White up-pointing triangle", "triangle delta", "Geometry"); Add("▲", "Black up-pointing triangle", "filled triangle", "Geometry");
        Add("▽", "White down-pointing triangle", "triangle nabla", "Geometry"); Add("▼", "Black down-pointing triangle", "filled down triangle", "Geometry");
        Add("◁", "White left-pointing triangle", "triangle", "Geometry"); Add("▷", "White right-pointing triangle", "triangle", "Geometry");
        Add("◀", "Black left-pointing triangle", "filled triangle", "Geometry"); Add("▶", "Black right-pointing triangle", "filled triangle", "Geometry");
        Add("○", "Circle", "circumference round", "Geometry"); Add("●", "Black circle", "filled circle disk", "Geometry");
        Add("◌", "Dotted circle", "dotted circle", "Geometry"); Add("◯", "Large circle", "empty circle", "Geometry");
        Add("⊖", "Circled minus", "circle minus", "Geometry"); Add("⌀", "Diameter sign", "diameter empty set", "Geometry");
        Add("⌒", "Arc", "arc geometry", "Geometry"); Add("⌢", "Frown arc", "arc chord", "Geometry");
        Add("⌣", "Smile arc", "arc", "Geometry"); Add("⌜", "Top left corner", "corner", "Geometry");
        Add("⌝", "Top right corner", "corner", "Geometry"); Add("⌞", "Bottom left corner", "corner", "Geometry");
        Add("⌟", "Bottom right corner", "corner", "Geometry"); Add("⦜", "Right angle", "geometry right angle", "Geometry");
        Add("⦝", "Measured right angle", "geometry angle", "Geometry"); Add("⦟", "Acute angle", "geometry", "Geometry");
        Add("⦢", "Spherical angle", "geometry", "Geometry"); Add("⦣", "Spherical angle", "geometry", "Geometry");
        Add("⦤", "Angle with s inside", "geometry angle", "Geometry"); Add("⦥", "Angle with l inside", "geometry angle", "Geometry");
        Add("⦦", "Circled angle", "geometry", "Geometry"); Add("⦧", "Reversed angle", "geometry", "Geometry");
        Add("∟", "Right angle", "perpendicular corner", "Geometry"); Add("⊥", "Perpendicular", "orthogonal 90 degrees", "Geometry");
        Add("∤", "Not parallel", "not perpendicular", "Geometry"); Add("⋈", "Bowtie", "bow tie geometric relation", "Geometry");
        Add("▱", "Parallelogram", "quadrilateral", "Geometry"); Add("▭", "Rectangle", "quadrilateral", "Geometry");
        Add("▢", "White square with rounded corners", "square", "Geometry"); Add("▪", "Small black square", "filled square", "Geometry");
        Add("▫", "Small white square", "empty square", "Geometry"); Add("⬠", "Pentagon", "five sided polygon", "Geometry");
        Add("⬡", "Hexagon", "six sided polygon", "Geometry"); Add("⬢", "Black hexagon", "filled hexagon", "Geometry");

        // Arrows and mappings
        Add("←", "Left arrow", "leftwards", "Arrows"); Add("↑", "Up arrow", "upwards", "Arrows");
        Add("→", "Right arrow", "rightwards maps to tends to", "Arrows"); Add("↓", "Down arrow", "downwards", "Arrows");
        Add("↔", "Left right arrow", "both directions", "Arrows"); Add("↕", "Up down arrow", "vertical both directions", "Arrows");
        Add("↖", "North west arrow", "upper left", "Arrows"); Add("↗", "North east arrow", "upper right", "Arrows");
        Add("↘", "South east arrow", "lower right", "Arrows"); Add("↙", "South west arrow", "lower left", "Arrows");
        Add("⇐", "Left double arrow", "implied by", "Arrows"); Add("⇑", "Up double arrow", "double upwards", "Arrows");
        Add("⇒", "Right double arrow", "implies if then", "Arrows"); Add("⇓", "Down double arrow", "double downwards", "Arrows");
        Add("⇔", "Left right double arrow", "if and only if iff equivalent", "Arrows"); Add("⇕", "Up down double arrow", "vertical iff", "Arrows");
        Add("⇖", "North west double arrow", "upper left", "Arrows"); Add("⇗", "North east double arrow", "upper right", "Arrows");
        Add("⇘", "South east double arrow", "lower right", "Arrows"); Add("⇙", "South west double arrow", "lower left", "Arrows");
        Add("↚", "Left arrow with stroke", "not left arrow", "Arrows"); Add("↛", "Right arrow with stroke", "not implies", "Arrows");
        Add("↮", "Left right arrow with stroke", "not both directions", "Arrows"); Add("↦", "Maps to", "mapsto function", "Arrows");
        Add("↤", "Leftwards arrow from bar", "maps from", "Arrows"); Add("↪", "Right arrow with hook", "injection", "Arrows");
        Add("↩", "Left arrow with hook", "return", "Arrows"); Add("↠", "Right arrow with two heads", "surjection onto", "Arrows");
        Add("↞", "Left arrow with two heads", "surjection", "Arrows"); Add("↣", "Right arrow with tail", "injection", "Arrows");
        Add("↢", "Left arrow with tail", "injection", "Arrows"); Add("↫", "Left arrow with loop", "arrow", "Arrows");
        Add("↬", "Right arrow with loop", "arrow", "Arrows"); Add("↭", "Left right wave arrow", "wave", "Arrows");
        Add("↝", "Right squiggle arrow", "leads to", "Arrows"); Add("↜", "Left squiggle arrow", "arrow", "Arrows");
        Add("↯", "Down zigzag arrow", "lightning", "Arrows"); Add("⇀", "Right harpoon up", "vector", "Arrows");
        Add("⇁", "Right harpoon down", "vector", "Arrows"); Add("⇄", "Right over left arrows", "exchange", "Arrows");
        Add("⇆", "Left over right arrows", "exchange", "Arrows"); Add("⇌", "Right harpoon over left harpoon", "equilibrium reaction", "Arrows");
        Add("⇋", "Left harpoon over right harpoon", "equilibrium", "Arrows"); Add("↼", "Left harpoon up", "vector", "Arrows");
        Add("↽", "Left harpoon down", "vector", "Arrows"); Add("↾", "Up harpoon right", "vector", "Arrows");
        Add("↿", "Up harpoon left", "vector", "Arrows"); Add("⇂", "Down harpoon right", "vector", "Arrows");
        Add("⇃", "Down harpoon left", "vector", "Arrows"); Add("⇝", "Right squiggle arrow", "long arrow", "Arrows");
        Add("⟵", "Long left arrow", "longwards", "Arrows"); Add("⟶", "Long right arrow", "long mapsto", "Arrows");
        Add("⟷", "Long left right arrow", "long both directions", "Arrows"); Add("⟸", "Long left double arrow", "long implied by", "Arrows");
        Add("⟹", "Long right double arrow", "long implies", "Arrows"); Add("⟺", "Long left right double arrow", "long iff", "Arrows");
        Add("⤳", "Rightwards arrow above tilde", "long leads to", "Arrows"); Add("⤺", "Leftwards arrow with loop", "curved arrow", "Arrows");
        Add("⤻", "Rightwards arrow with loop", "curved arrow", "Arrows"); Add("↗", "Northeast arrow", "tends to infinity", "Arrows");
        Add("⤴", "Right arrow curving up", "curve arrow", "Arrows"); Add("⤵", "Right arrow curving down", "curve arrow", "Arrows");

        // Greek letters (upper and lower case)
        string[,] greek = {
            {"Α","Alpha","alpha"},{"Β","Beta","beta"},{"Γ","Gamma","gamma"},{"Δ","Delta","delta change"},{"Ε","Epsilon","epsilon"},{"Ζ","Zeta","zeta"},{"Η","Eta","eta"},{"Θ","Theta","theta"},{"Ι","Iota","iota"},{"Κ","Kappa","kappa"},{"Λ","Lambda","lambda"},{"Μ","Mu","mu"},{"Ν","Nu","nu"},{"Ξ","Xi","xi"},{"Ο","Omicron","omicron"},{"Π","Pi","pi"},{"Ρ","Rho","rho"},{"Σ","Sigma","sigma sum"},{"Τ","Tau","tau"},{"Υ","Upsilon","upsilon"},{"Φ","Phi","phi"},{"Χ","Chi","chi"},{"Ψ","Psi","psi"},{"Ω","Omega","omega"},
            {"α","alpha","alpha"},{"β","beta","beta"},{"γ","gamma","gamma"},{"δ","delta","delta change difference"},{"ε","epsilon","epsilon"},{"ζ","zeta","zeta"},{"η","eta","eta"},{"θ","theta","theta"},{"ι","iota","iota"},{"κ","kappa","kappa"},{"λ","lambda","lambda"},{"μ","mu","mu micro"},{"ν","nu","nu"},{"ξ","xi","xi"},{"ο","omicron","omicron"},{"π","pi","pi"},{"ρ","rho","rho"},{"σ","sigma","sigma"},{"ς","final sigma","final sigma"},{"τ","tau","tau"},{"υ","upsilon","upsilon"},{"φ","phi","phi"},{"χ","chi","chi"},{"ψ","psi","psi"},{"ω","omega","omega"},
            {"ϐ","beta symbol","curly beta beta variant"},{"ϑ","theta symbol","vartheta theta variant"},{"ϕ","phi symbol","varphi phi variant"},{"ϖ","pi symbol","varpi pi variant"},{"ϱ","rho symbol","varrho rho variant"},{"ϵ","epsilon symbol","varepsilon epsilon variant"},{"Ϝ","Digamma","digamma stigma"},{"ϝ","digamma","digamma stigma"}
        };
        for (int i = 0; i < greek.GetLength(0); i++) Add(greek[i, 0], greek[i, 1], greek[i, 2], "Greek");

        // Fractions, superscripts, subscripts, and common special characters
        Add("¼", "One quarter", "one fourth fraction", "Other"); Add("½", "One half", "half fraction", "Other");
        Add("¾", "Three quarters", "three fourths fraction", "Other"); Add("⅐", "One seventh", "fraction", "Other");
        Add("⅑", "One ninth", "fraction", "Other"); Add("⅒", "One tenth", "fraction", "Other");
        Add("⅓", "One third", "fraction", "Other"); Add("⅔", "Two thirds", "fraction", "Other");
        Add("⅕", "One fifth", "fraction", "Other"); Add("⅖", "Two fifths", "fraction", "Other");
        Add("⅗", "Three fifths", "fraction", "Other"); Add("⅘", "Four fifths", "fraction", "Other");
        Add("⅙", "One sixth", "fraction", "Other"); Add("⅚", "Five sixths", "fraction", "Other");
        Add("⅛", "One eighth", "fraction", "Other"); Add("⅜", "Three eighths", "fraction", "Other");
        Add("⅝", "Five eighths", "fraction", "Other"); Add("⅞", "Seven eighths", "fraction", "Other");
        string supers = "⁰|superscript zero|superscript 0;¹|superscript one|superscript 1;²|superscript two|superscript 2;³|superscript three|superscript 3;⁴|superscript four|superscript 4;⁵|superscript five|superscript 5;⁶|superscript six|superscript 6;⁷|superscript seven|superscript 7;⁸|superscript eight|superscript 8;⁹|superscript nine|superscript 9;⁺|superscript plus|superscript addition;⁻|superscript minus|superscript subtraction;⁼|superscript equals|superscript equal;⁽|superscript left parenthesis|superscript bracket;⁾|superscript right parenthesis|superscript bracket;ⁿ|superscript n|power exponent;ⁱ|superscript i|power exponent;₀|subscript zero|subscript 0;₁|subscript one|subscript 1;₂|subscript two|subscript 2;₃|subscript three|subscript 3;₄|subscript four|subscript 4;₅|subscript five|subscript 5;₆|subscript six|subscript 6;₇|subscript seven|subscript 7;₈|subscript eight|subscript 8;₉|subscript nine|subscript 9;₊|subscript plus|subscript addition;₋|subscript minus|subscript subtraction;₌|subscript equals|subscript equal;₍|subscript left parenthesis|subscript bracket;₎|subscript right parenthesis|subscript bracket;ₐ|subscript a|subscript letter;ₑ|subscript e|subscript letter;ₕ|subscript h|subscript letter;ᵢ|subscript i|subscript letter;ⱼ|subscript j|subscript letter;ₖ|subscript k|subscript letter;ₗ|subscript l|subscript letter;ₘ|subscript m|subscript letter;ₙ|subscript n|subscript letter;ₒ|subscript o|subscript letter;ₚ|subscript p|subscript letter;ᵣ|subscript r|subscript letter;ₛ|subscript s|subscript letter;ₜ|subscript t|subscript letter;ᵤ|subscript u|subscript letter;ᵥ|subscript v|subscript letter;ₓ|subscript x|subscript letter";
        foreach (var entry in supers.Split(';')) { var p = entry.Split('|'); Add(p[0], p[1], p[2], "Other"); }
        Add("ℕ", "Natural numbers", "natural number set blackboard bold n", "Algebra");
        Add("ℤ", "Integers", "integer number set blackboard bold z", "Algebra");
        Add("ℚ", "Rational numbers", "rational number set blackboard bold q", "Algebra");
        Add("ℝ", "Real numbers", "real number set blackboard bold r", "Algebra");
        Add("ℂ", "Complex numbers", "complex number set blackboard bold c", "Algebra");
        Add("ℙ", "Prime numbers", "prime probability set", "Algebra");
        Add("ℍ", "Quaternions", "quaternion number set", "Algebra"); Add("ℵ", "Aleph", "aleph cardinality infinity", "Algebra");
        Add("ℶ", "Beth", "beth cardinality", "Algebra"); Add("ℷ", "Gimel", "gimel cardinality", "Algebra");
        Add("℘", "Weierstrass p", "script p power set", "Algebra"); Add("ℑ", "Imaginary part", "imaginary part", "Algebra");
        Add("ℜ", "Real part", "real part", "Algebra"); Add("ℱ", "Fourier transform", "fourier script f", "Calculus");
        Add("ℒ", "Laplace transform", "laplace script l", "Calculus"); Add("ℯ", "Euler's number", "e exponential natural logarithm", "Algebra");
        Add("ⅈ", "Imaginary unit", "i imaginary number", "Algebra"); Add("ⅉ", "Imaginary unit j", "j imaginary number", "Algebra");
        Add("∅", "Empty set", "empty null set", "Sets & Logic"); Add("∶", "Ratio", "ratio colon", "Algebra");
        Add("∷", "Proportion", "as is to proportion", "Algebra"); Add("∴", "Therefore", "therefore", "Other");
        Add("∵", "Because", "because", "Other"); Add("※", "Reference mark", "asterisk reference", "Other");
        Add("§", "Section sign", "section", "Other"); Add("¶", "Paragraph sign", "paragraph", "Other");
        Add("©", "Copyright", "copyright", "Other"); Add("®", "Registered", "registered trademark", "Other");
        Add("™", "Trademark", "trademark", "Other"); Add("℃", "Degrees Celsius", "celsius temperature", "Other");
        Add("℉", "Degrees Fahrenheit", "fahrenheit temperature", "Other"); Add("µ", "Micro sign", "micro prefix", "Other");
        Add("Ω", "Ohm", "ohm resistance omega", "Other"); Add("℧", "Mho", "conductance siemens", "Other");
        Add("Å", "Angstrom", "angstrom length", "Other"); Add("‰", "Per mille", "per thousand", "Other");
        Add("♯", "Sharp", "music sharp", "Other"); Add("♭", "Flat", "music flat", "Other");

        // Handy relation variants and math block operators
        string[] extra = {
            "≉|Not approximately equal|not approx|Relations", "≊|Approximately equal or equal to|approx|Relations", "≋|Triple tilde|triple similar|Relations",
            "≌|All equal to|all equal|Relations", "≍|Equivalent to|equivalent|Relations", "≎|Geometrically equivalent to|geometry equivalent|Geometry",
            "≏|Difference between|difference|Relations", "≖|Ring in equal to|ring equal|Relations", "≗|Ring equal to|ring equal|Relations",
            "≘|Corresponds to|corresponds|Relations", "≙|Estimates|estimates|Relations", "≚|Equiangular to|equiangular|Geometry",
            "≛|Star equals|star equal|Relations", "≞|Measured by|measured|Relations", "≟|Questioned equal|question equal|Relations",
            "≪|Much less than|much less|Relations", "≫|Much greater than|much greater|Relations", "⋘|Very much less than|very much less|Relations",
            "⋙|Very much greater than|very much greater|Relations", "⋚|Less than equal or greater than|less equal greater|Relations", "⋛|Greater than equal or less than|greater equal less|Relations",
            "⋜|Equal to or less than|equal less|Relations", "⋝|Equal to or greater than|equal greater|Relations", "⋞|Equal to or precedes|precedes|Relations",
            "⋟|Equal to or succeeds|succeeds|Relations", "⋠|Does not precede or equal|not precedes|Relations", "⋡|Does not succeed or equal|not succeeds|Relations",
            "⋢|Not square image of or equal to|not square|Relations", "⋣|Square image of or not equal to|square|Relations", "⋤|Square image of or equal to|square|Relations",
            "⋥|Square original of or equal to|square|Relations", "⋦|Less than but not equivalent|less equivalent|Relations", "⋧|Greater than but not equivalent|greater equivalent|Relations",
            "⋨|Precedes but not equivalent|precedes|Relations", "⋩|Succeeds but not equivalent|succeeds|Relations", "⋪|Not normal subgroup of|not subgroup|Algebra",
            "⋫|Does not contain as normal subgroup|not subgroup|Algebra", "⋬|Not normal subgroup or equal|not subgroup|Algebra", "⋭|Does not contain normal subgroup or equal|not subgroup|Algebra",
            "⋮|Vertical ellipsis|three dots vertical|Other", "⋯|Midline horizontal ellipsis|three dots horizontal|Other", "⋰|Up right diagonal ellipsis|diagonal dots|Other",
            "⋱|Down right diagonal ellipsis|diagonal dots|Other", "⋲|Element of with long horizontal stroke|element set|Sets & Logic", "⋳|Element of with vertical bar|element set|Sets & Logic",
            "⋴|Small element of with vertical bar|element set|Sets & Logic", "⋵|Element of with dot above|element set|Sets & Logic", "⋶|Element of with overbar|element set|Sets & Logic",
            "⋷|Small element of with overbar|element set|Sets & Logic", "⋸|Element of with underbar|element set|Sets & Logic", "⋹|Element of with two horizontal strokes|element set|Sets & Logic",
            "⋺|Contains with long horizontal stroke|contains set|Sets & Logic", "⋻|Contains with vertical bar|contains set|Sets & Logic", "⋼|Small contains with vertical bar|contains set|Sets & Logic",
            "⋽|Contains with overbar|contains set|Sets & Logic", "⋾|Small contains with overbar|contains set|Sets & Logic", "⋿|Z notation bag membership|bag membership|Sets & Logic",
            "⨀|N-ary circled dot|large circled dot|Algebra", "⨁|N-ary circled plus|large circled plus|Algebra", "⨂|N-ary circled times|large circled times|Algebra",
            "⨄|N-ary union operator|large union|Sets & Logic", "⨆|N-ary square union|large square union|Sets & Logic", "⨇|Two logical and|big wedge|Sets & Logic",
            "⨈|Two logical or|big vee|Sets & Logic", "⨉|N-ary times operator|large times|Algebra", "⨊|Summation with integral|sum integral|Calculus",
            "⨋|Quadruple integral operator|quadruple integral|Calculus", "⨍|Finite part integral|finite part|Calculus", "⨎|Integral with double stroke|integral|Calculus",
            "⨏|Integral average with slash|average integral|Calculus", "⨐|Integral around a point|contour integral|Calculus", "⨑|Integral around a point clockwise|contour integral|Calculus",
            "⨒|Integral around a union|contour|Calculus", "⨓|Integral around an intersection|contour|Calculus", "⨔|Integral with leftwards arrow|integral|Calculus",
            "⨕|Integral with rightwards arrow|integral|Calculus", "⨖|Integral with double arrow|integral|Calculus", "⨗|Integral with left arrow|integral|Calculus",
            "⨘|Integral with loop|integral|Calculus", "⨙|Integral with union|integral|Calculus", "⨚|Integral with intersection|integral|Calculus",
            "⨛|Integral with overbar|integral|Calculus", "⨜|Integral with underbar|integral|Calculus", "⨝|Join|relational join|Algebra",
            "⨞|Large left triangle operator|triangle operator|Algebra", "⨟|Z notation schema composition|schema|Algebra", "⨠|Z notation schema piping|schema|Algebra",
            "⨢|Plus sign with small circle above|plus|Algebra", "⨣|Plus sign with circumflex accent|plus|Algebra", "⨤|Plus sign with tilde above|plus|Algebra",
            "⨥|Plus sign with dot below|plus|Algebra", "⨦|Plus sign with tilde below|plus|Algebra", "⨧|Plus sign with subscript two|plus|Algebra",
            "⨪|Minus sign with comma above|minus|Algebra", "⨫|Minus sign with dot below|minus|Algebra", "⨬|Minus sign with falling dots|minus|Algebra",
            "⨭|Minus sign with rising dots|minus|Algebra", "⨮|Plus sign in left half circle|plus|Algebra", "⨰|Multiplication sign with dot above|times|Algebra",
            "⨱|Multiplication sign with underbar|times|Algebra", "⨲|Semidirect product with bottom closed|semidirect product|Algebra", "⨳|Smash product|smash|Algebra",
            "⨴|Multiplication sign in left half circle|times|Algebra", "⨵|Multiplication sign in right half circle|times|Algebra", "⨶|Circled multiplication sign with circumflex|times|Algebra",
            "⨷|Multiplication sign in double circle|times|Algebra", "⨸|Circled division sign|division|Algebra", "⨹|Plus sign in triangle|plus|Algebra",
            "⨺|Minus sign in triangle|minus|Algebra", "⨻|Multiplication sign in triangle|times|Algebra", "⨼|Interior product|interior product|Algebra",
            "⨽|Righthand interior product|interior product|Algebra", "⨾|Z notation relational composition|relation composition|Algebra", "⨿|Amalgamation or coproduct|amalgamation|Algebra",
            "⋀|N-ary logical and|big wedge|Sets & Logic", "⋁|N-ary logical or|big vee|Sets & Logic", "⋂|N-ary intersection|big cap|Sets & Logic",
            "⋃|N-ary union|big cup|Sets & Logic", "⋄|Diamond operator|lozenge|Algebra", "⋅|Dot operator|dot|Algebra", "⋆|Star operator|star|Algebra",
            "⋇|Division times|division times|Algebra", "⋈|Bowtie|join|Algebra", "⋉|Left normal factor semidirect product|semidirect|Algebra",
            "⋊|Right normal factor semidirect product|semidirect|Algebra", "⋋|Left semidirect product|semidirect|Algebra", "⋌|Right semidirect product|semidirect|Algebra",
            "⋍|Reversed tilde equals|similar|Relations", "⋎|Curly logical or|or|Sets & Logic", "⋏|Curly logical and|and|Sets & Logic",
            "⋐|Double subset|subset|Sets & Logic", "⋑|Double superset|superset|Sets & Logic", "⋒|Double intersection|intersection|Sets & Logic",
            "⋓|Double union|union|Sets & Logic", "⋔|Pitchfork|meet|Sets & Logic", "⋕|Equal and parallel to|parallel|Geometry",
            "⋖|Less than with dot|less than|Relations", "⋗|Greater than with dot|greater than|Relations"
        };
        foreach (var line in extra)
        {
            var p = line.Split('|');
            Add(p[0], p[1], p[2], p[3]);
        }

        items.AddRange(UnicodeSymbolData.Entries);
        return items
            .GroupBy(item => item.Glyph)
            .Select(group =>
            {
                var first = group.First();
                string mergedTerms = string.Join(" ", group.SelectMany(item => new[] { item.Name, item.SearchTerms, item.Category }).Distinct());
                return first with { SearchTerms = mergedTerms };
            })
            .ToList();
    }
}
