# Overview

This project is a fully integer-based calculator supporting:

- Classic operations: + - * /

- Unary operators and sign handling

- BigInteger arithmetic (unlimited number size)

- Operator precedence (× and ÷ before + and -)

- Detailed error reporting

- Async batch file processing for large files (10GB+)

It is built as a portfolio-quality project showcasing clean architectural design, separation of concerns, WPF UI work, and production-level error handling.

## Main Features
### Interactive Calculator
- **Basic Operations**: Addition (+), Subtraction (-), Multiplication (×), Division (÷)
- **Operator Precedence**: Follows standard mathematical rules (× and ÷ before + and -)
- **Unary Operators**: Supports negative numbers and sign handling (e.g., `-3`, `2 + -5`)
- **Large Numbers**: Uses `BigInteger` for unlimited precision (no overflow)
- **Real-time Validation**: Immediate feedback on invalid expressions

### Batch File Processing
- **Async File Operations**: Non-blocking file I/O with streaming
- **Large File Support**: Efficiently handles files of any size through lazy evaluation
- **Error Resilience**: Continues processing even when individual expressions fail
- **Detailed Output**: Each line in output corresponds to input with results or error messages
- **Custom Output Names**: Optional custom naming for output files

### User Experience
- **Modern Dark Theme**: Professional, eye-friendly interface
- **Responsive UI**: Never freezes during long operations
- **Clear Error Messages**: Descriptive feedback for all error scenarios
- **Intuitive Layout**: Calculator-style button grid with file processing controls

## Architecture

### Multi-Layer Architecture

The application follows a **layered architecture** pattern with clear separation of concerns:
```
Presentation (WPF)
AppLayer (Orchestration)
Core (Tokenizer → Normalizer → Validator → Evaluator)
IO (Async file operations)
Tests( xUnit tests)
```
### Key Architectural Decisions

**1. Why Multi-Project Solution?**
- **Separation of Concerns**: Each layer has distinct responsibility
- **Reusability**: Core logic can be used in console, web, or mobile apps
- **Testability**: Easy to test each layer independently
- **Maintainability**: Changes in UI don't affect business logic

**2. Why Separate Normalization Steps?**
- **Single Responsibility**: Each normalizer does one thing well
- **Debuggability**: Easy to trace which step caused an issue
- **Flexibility**: Easy to add/remove normalization rules - for example adding new operators, brackets etc..
- **Testing**: Each step can be unit tested independently

**3. Why BigInteger?**
- **No Overflow**: Handles numbers of any size
- **Accuracy**: Integer division without floating-point errors

**4. Why Async File Processing?**
- **Responsiveness**: UI never freezes
- **Scalability**: Handles 10GB+ files without memory issues
- **Best Practice**: Modern I/O should always be async

### Design Patterns Used

#### 1. **MVVM (Model-View-ViewModel)**
- **Purpose**: Separation of UI from business logic
- **Implementation**: 
  - Views are pure XAML with no code-behind logic
  - ViewModels handle all UI state and user interactions
  - Two-way data binding via `INotifyPropertyChanged`
  - Commands for user actions (`RelayCommand`, `AsyncRelayCommand`)

#### 2. **Dependency Injection**
- **Purpose**: Loose coupling and testability
- **Implementation**: Microsoft.Extensions.DependencyInjection
- **Benefits**:
  - Easy unit testing through interface mocking
  - Single Responsibility Principle adherence
  - Runtime flexibility

#### 3. **Strategy Pattern**
- **Purpose**: Interchangeable processing algorithms
- **Implementation**: 
  - `IExpressionTokenizer` - converts string to tokens
  - `IExpressionNormalizer` - normalizes operator sequences
  - `IExpressionValidator` - validates syntax
  - `IExpressionEvaluator` - computes result

#### 4. **Pipeline Pattern**
- **Purpose**: Sequential processing stages
- **Implementation**: Expression evaluation flows through:
  1. Tokenization
  2. Normalization
  3. Validation
  4. Evaluation

#### 5. **Repository Pattern (I/O Layer)**
- **Purpose**: Abstraction of data access
- **Implementation**: `IFileService` abstracts file operations

Each layer is isolated via interfaces and registered through Dependency Injection, ensuring testability and maintainability.

## Technical Implementation

### Expression Evaluation Pipeline

The calculator processes mathematical expressions through a sophisticated multi-stage pipeline:

#### **Stage 1: Tokenization**
```
Input:  "2 + -3 * 2"
Output: [Number(2), Operator(+), Operator(-), Number(3), Operator(*), Number(2)]
```

**Tokenizer** (`ExpressionTokenizer`):
- Scans input character by character
- Identifies numeric sequences and operators
- Throws `UnsupportedCharacterException` for invalid characters (letters, decimals)
- Ignores whitespace

#### **Stage 2: Normalization**
```
Before: [Number(2), Operator(+), Operator(-), Number(3), Operator(*), Number(2)]
After:  [Number(2), Operator(+), Unary(-), Number(3), Operator(*), Number(2)]
```

**Normalizer** (`ExpressionNormalizer`) performs three sub-steps:

1. **Sign Normalization** (`SignNormalizer`):
   - Reduces consecutive sign operators: `--` → `+`, `+-` → `-`
   - Handles complex sequences: `---` → `-`, `+--` → `+`

2. **Unary Classification** (`UnaryClassifier`):
   - Marks operators at start or after operators as unary
   - Distinguishes `2 - 3` (binary) from `2 * -3` (unary)

3. **Unary Merging** (`UnaryMerger`):
   - Merges unary operators with their operands
   - Converts `Unary(-), Number(3)` → `Number(-3)`

#### **Stage 3: Validation**
```
Checks:
✓ Expression not empty
✓ No operator at end
✓ No invalid operator sequences
✓ Proper unary placement
```

**Validator** (`ExpressionValidator`):
- Ensures expression doesn't end with operator
- Validates operator sequences (e.g., `**` is invalid)
- Verifies unary operators are properly placed
- Returns descriptive error messages

#### **Stage 4: Evaluation**
```
Expression: 2 + -3 * 2
Process:    2 + (-6)  (multiplication first due to precedence)
Result:     -4
```

**Evaluator** (`ExpressionEvaluator`):
- Implements operator precedence (× ÷ before + -)
- Uses `BigInteger` for unlimited number size
- Handles division by zero
- Returns result or error


### Exception Handling Strategy

**Three-Layer Protection**:

1. **Preventive Validation** (Application Layer):
   ```csharp
   // Validate before processing
   if (string.IsNullOrWhiteSpace(inputPath))
       return Fail("Input file path is empty", ErrorType.Warning);
   ```

2. **Graceful Exception Catching** (Core Layer):
   ```csharp
   try {
       var result = _expressionEvaluator.Evaluate(expression);
   } catch (UnsupportedCharacterException ex) {
       return new ParsingResult { 
           Success = false, 
           ErrorMessage = ex.Message,
           ErrorType = ErrorTypeCore.Error 
       };
   }
   ```

3. **Global Exception Handler** (Presentation Level):
   ```csharp
   // Catches ALL unhandled exceptions
   Application.DispatcherUnhandledException
   TaskScheduler.UnobservedTaskException
   AppDomain.UnhandledException
   ```

**No crashes allowed** - Every exception path leads to user-friendly error message.

---

## File Processing

### Workflow

```
┌──────────────┐
│ User selects │
│  input file  │
└──────┬───────┘
       │
       ▼
┌──────────────────┐
│ Validation Layer │
│ • Path format    │
│ • File exists    │
│ • Extension .txt │
│ • Permissions    │
└──────┬───────────┘
       │
       ▼
┌──────────────────┐
│  Lazy Reading    │
│ IAsyncEnumerable │
│ (line by line)   │
└──────┬───────────┘
       │
       ▼
┌──────────────────┐
│  For Each Line:  │
│ • Skip empty     │
│ • Evaluate expr  │
│ • Collect result │
└──────┬───────────┘
       │
       ▼
┌──────────────────┐
│  Write Results   │
│ • Async I/O      │
│ • Buffered       │
│ • UTF-8          │
└──────────────────┘
```
### File Processing Mode

**Setup**:
1. Click "Browse..." next to "Input file" or type path directly into TextBox
2. Select your `.txt` file
3. Click "Browse..." next to "Output directory" or type path directly into TextBox
4. Select destination folder
5. (Optional) Enter custom output filename
6. Click "Evaluate from file"

**What Happens**:
- Progress spinner appears
- File processed asynchronously
- Success message on completion
- Output file created in selected directory

**Output Naming**:
- Custom name: `results.txt`
- Auto-generated: `Output_20241130_143022.txt`
### Performance Optimizations

**Large File Handling** (10GB+ support):

1. **Streaming with `IAsyncEnumerable`**:
   ```csharp
   await foreach (var line in _fileService.GetFileLinesAsync(filePath)) {
       // Process one line at a time - never loads entire file
   }
   ```

2. **Buffered Writing**:
   ```csharp
   new FileStream(path, FileMode.Create, 
                  FileAccess.Write, FileShare.None,
                  bufferSize: 8192,  // 8KB buffer
                  useAsync: true)
   ```

3. **Memory Efficiency**:
   - No List accumulation for input lines
   - Output lines collected incrementally
   - Each expression evaluated independently

**Example**: Processing 10GB file with 1M lines:
- **Processing**: One line at a time, no datasets are stored into memory
- **UI**: Remains responsive (async/await)

## Getting Started

**Clone the repo:**
```bash
git clone https://github.com/BigChillingSpoon/Calculator
cd Calculator
```

**Run:**
```bash
dotnet run --project Calculator/Calculator.csproj
```
## Screenshots
<img width="440" height="603" alt="Snímek obrazovky 2025-12-01 171324" src="https://github.com/user-attachments/assets/ea434b5f-e515-4352-bf85-b8c48930869a" />
<img width="434" height="599" alt="Snímek obrazovky 2025-12-01 171408" src="https://github.com/user-attachments/assets/58fa4ac0-5f33-48c7-9181-cee3a80b7634" />
<img width="1268" height="602" alt="Snímek obrazovky 2025-12-01 171010" src="https://github.com/user-attachments/assets/4049d18c-423b-4412-b0b8-9201710be66b" />
<img width="433" height="602" alt="Snímek obrazovky 2025-12-01 172155" src="https://github.com/user-attachments/assets/d2845516-6e13-454b-8f4b-2b041438692b" />


## Future Enhancements
- Custom tile bar + message box
  
- Parentheses support

- Additional operators (^, %)

- Keyboard input support

- Light theme

- Localization

- Cancel option in time consuming operations
