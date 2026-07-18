using AIKnowledgeBot.InterFace.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBot.Services.Search
{
    public class SimilarityService : ISimilarityService
    {
        public double CosineSimilarity(float[] vector1, float[] vector2)
        {
            if (vector1.Length != vector2.Length)
                throw new ArgumentException("Vectors must have the same dimension.");

            double dot = 0;
            double magnitudeA = 0;
            double magnitudeB = 0;

            for (int i = 0; i < vector1.Length; i++)
            {
                dot += vector1[i] * vector2[i];
                magnitudeA += vector1[i] * vector1[i];
                magnitudeB += vector2[i] * vector2[i];
            }

            if (magnitudeA == 0 || magnitudeB == 0)
                return 0;

            return dot / (Math.Sqrt(magnitudeA) * Math.Sqrt(magnitudeB));
        }
    }
}
