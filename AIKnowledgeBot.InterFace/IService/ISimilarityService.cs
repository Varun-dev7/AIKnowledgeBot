using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBot.InterFace.IService
{
    public interface ISimilarityService
    {
        double CosineSimilarity(float[] vector1, float[] vector2);
    }
}
