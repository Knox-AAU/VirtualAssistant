﻿using System.Linq;
using VirtualAssistantBusinessLogic.SparQL;
using System.Collections.Generic;

namespace VirtualAssistantBusinessLogic.KnowledgeGraph
{
    public class KnowledgeGraph
    {
        public KnowledgeGraphNode FindNodeInformation(KnowledgeGraphNode node)
        {
            SparQLConnection sparqlConnection = new SparQLConnection();
            SparQLBuilder sparqlBuilder = GetSparQLBuilder(node.Information["Type"][0]);
            sparqlBuilder.Query = node.Id;
            string sparqlQuery = sparqlBuilder.ToString();
            var results = sparqlConnection.ExecuteQuery(sparqlQuery, new XMLResponseDecoder());

            //Since we are looking for information of a specific node, we know that there is only one result
            var result = results.FirstOrDefault();
            node.Information = result.Value;
            return node;
        }

        public List<KnowledgeGraphNode> FindNodes(string query)
        {
            SparQLConnection sparqlConnection = new SparQLConnection();
            SparQLBuilder sparqlBuilder = GetSparQLBuilder("");
            sparqlBuilder.Query = query;
            string sparqlQuery = sparqlBuilder.ToString();
            var results = sparqlConnection.ExecuteQuery(sparqlQuery, new XMLResponseDecoder());

            List<KnowledgeGraphNode> nodeList = new List<KnowledgeGraphNode>();
            foreach (var kvp in results)
            {
                KnowledgeGraphNode node = new KnowledgeGraphNode();
                node.Id = kvp.Key;
                //TODO figure out how to get the nodes own name (fx The node of "Barack Obama's wife" should be called "Michele Obama")
                node.Information = kvp.Value;

                nodeList.Add(node);
            }
            
            return nodeList;
        }

        private SparQLBuilder GetSparQLBuilder(string type)
        {
            switch (type)
            {
                case "": return new UnknownSparQLBuilder();
                case "Human": return new PersonSparQLBuilder();
                default: return new SparQLBuilder();
            }
        }
    }
}
