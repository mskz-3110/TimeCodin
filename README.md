# TimeCodin
Analyzing time code from audio data

## Unity
`https://github.com/mskz-3110/TimeCodin.git?path=/Unity/TimeCodin/Assets/TimeCodin/Runtime#v1.0.0`

|Version|Summary|
|:--|:--|
|1.0.0|Initial version|

## Usage
```cs
using TimeCodin;

var ltcAnalyzer = new LtcAnalyzer();
ltcAnalyzer.Analyze(/* float[] type audio data */, 1, (in TimeCode timeCode) => {
  // Analyzed time code
});
```
