using Microsoft.ML;

namespace JobFitScoreAPI.Services
{
    public class JobFitMlService
    {
        private readonly MLContext _ml;

        public JobFitMlService(MLContext ml)
        {
            _ml = ml;
        }
    }
}
