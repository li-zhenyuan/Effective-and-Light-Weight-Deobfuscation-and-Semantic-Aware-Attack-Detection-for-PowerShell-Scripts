using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Core.Data;

namespace PowershellDeobfuscation
{
    public class Classifier
    {
        private string modelPath = "Data\\ObfuscationClassifierModel.zip";

        public enum ClassifierResult { unobfuscated, obfuscated };


        static PredictionEngine<AstData, AstPrediction> predEngine;

        public void initPredEngine()
        {
            MLContext mlContext = new MLContext(95, 0);

            ITransformer trainedModel;
            using (var stream = new FileStream(modelPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                trainedModel = mlContext.Model.Load(stream);
            }
            predEngine = trainedModel.CreatePredictionEngine<AstData, AstPrediction>(mlContext);
        }

        public void initPredEngine(string modelPath)
        {
            this.modelPath = modelPath;
            initPredEngine();
        }

        public ClassifierResult testWithModel(AstData sampleData)
        {
            AstPrediction result = predEngine.Predict(sampleData);

            int count = 0;
            foreach (var p in result.Score)
            {
                Console.Out.WriteLine("{0} -- {1}", count++, p);
            }

            if (result.Score[0] > 0.5)
                return ClassifierResult.unobfuscated;
            else
                return ClassifierResult.obfuscated;
        }
    }

    public class AstPrediction
    {
        public float[] Score;
    }
}
