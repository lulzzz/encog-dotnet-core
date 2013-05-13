﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.EA.Genome;
using Encog.Util;
using Encog.ML.EA.Species;


namespace Encog.ML.EA.Population
{
    /// <summary>
    /// Defines the basic functionality for a population of genomes. The population
    /// is made up of species. These species contain the individiual genomes that
    /// make up the population. If you do not want to use species, then create one
    /// species that holds every genome.
    /// </summary>
    [Serializable]
    public class BasicPopulation : BasicML, IPopulation
    {
        /// <summary>
        /// The name of this object.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The object name.
        /// </summary>
        private String name;

        /// <summary>
        /// The species that make up the population.
        /// </summary>
        private List<ISpecies> species = new List<ISpecies>();

        /// <summary>
        /// The best genome.
        /// </summary>
        public IGenome BestGenome { get; set; }

        /// <summary>
        /// A factory that can be used to store create genomes.
        /// </summary>
        public IGenomeFactory GenomeFactory { get; set; }

        /// <summary>
        /// How many genomes should be created.
        /// </summary>
        public int PopulationSize { get; set; }

        /// <summary>
        /// Construct an empty population.
        /// </summary>
        public BasicPopulation()
        {
            PopulationSize = 0;
        }

        /// <summary>
        /// Construct a population.
        /// </summary>
        /// <param name="thePopulationSize">The population size.</param>
        /// <param name="theGenomeFactory">The genome factory.</param>
        public BasicPopulation(int thePopulationSize,
                IGenomeFactory theGenomeFactory)
        {
            PopulationSize = thePopulationSize;
            GenomeFactory = theGenomeFactory;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            this.species.Clear();

        }

        /// <inheritdoc/>
        public ISpecies CreateSpecies()
        {
            ISpecies species = new BasicSpecies();
            species.Population = this;
            this.species.Add(species);
            return species;
        }

        /// <inheritdoc/>
        public ISpecies DetermineBestSpecies()
        {
            foreach (ISpecies species in this.species)
            {
                if (species.Members.Contains(BestGenome))
                {
                    return species;
                }
            }
            return null;
        }

        /// <inheritdoc/>
        public IList<IGenome> Flatten()
        {
            IList<IGenome> result = new List<IGenome>();
            foreach (ISpecies species in this.species)
            {
                result = result.Union(species.Members).ToList();
            }
            return result;
        }


        /// <inheritdoc/>
        public int MaxIndividualSize
        {
            get
            {
                return int.MaxValue;
            }
        }

        /// <inheritdoc/>
        public int Count
        {
            get
            {
                return Flatten().Count;
            }
        }

        /// <inheritdoc/>
        public override void UpdateProperties()
        {

        }

        /// <inheritdoc/>
        public List<ISpecies> Species
        {
            get { return this.species; }
        }

        /// <summary>
        /// Purge invalid genomes.
        /// </summary>
        public void PurgeInvalidGenomes()
        {
            // remove any invalid genomes
            int speciesNum = 0;
            while (speciesNum < Species.Count)
            {
                ISpecies species = Species[speciesNum];

                int genomeNum = 0;
                while (genomeNum < species.Members.Count)
                {
                    IGenome genome = species.Members[genomeNum];
                    if (double.IsInfinity(genome.Score)
                            || double.IsInfinity(genome.AdjustedScore)
                            || double.IsNaN(genome.Score)
                            || double.IsNaN(genome.AdjustedScore))
                    {
                        species.Members.Remove(genome);
                    }
                    else
                    {
                        genomeNum++;
                    }
                }

                // is the species now empty?
                if (species.Members.Count == 0)
                {
                    Species.Remove(species);
                }
                else
                {
                    // new leader needed?
                    if (!species.Members.Contains(species.Leader))
                    {
                        species.Leader = species.Members[0];
                        species.BestScore = species.Leader.AdjustedScore;
                    }

                    // onto the next one!
                    speciesNum++;
                }
            }
        }
    }
}
