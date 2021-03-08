using UnityEngine;
using System.Collections;

namespace GridPathfindingSystem {

    public class BinaryTree {

        private class BinaryNode {
            public PathNode pathNode;
            public BinaryNode left;
            public BinaryNode right;

            public BinaryNode(PathNode _pathNode, BinaryNode _left, BinaryNode _right) {
                //id = nextId++;
                pathNode = _pathNode;
                left = _left;
                right = _right;
            }
        }

        private BinaryNode root;
        public int totalNodes = 0;

        public BinaryTree() {
            root = null;
        }

        public void AddNode(PathNode pathNode) {
            if (root == null) {
                root = new BinaryNode(pathNode, null, null);
                totalNodes++;
            } else {
                try {
                    addNode(root, pathNode, pathNode.fValue);
                } catch (System.Exception e) {
                    Debug.Log(e);
                }
                totalNodes++;
            }
        }

        private void addNode(BinaryNode node, PathNode pathNode, int pathFvalue) {
            if (pathFvalue <= node.pathNode.fValue) {
                if (node.left == null) {
                    node.left = new BinaryNode(pathNode, null, null);
                } else {
                    addNode(node.left, pathNode, pathFvalue);
                }
            } else {
                if (node.right == null) {
                    node.right = new BinaryNode(pathNode, null, null);
                } else {
                    addNode(node.right, pathNode, pathFvalue);
                }
            }
        }

        public void RemoveNode(PathNode pathNode) {
            if (root.pathNode == pathNode) {
                BinaryNode prevRoot = root;
                if (root.left == null && root.right == null) {
                    root = null;
                } else {
                    if (root.left == null && root.right != null) {
                        root = root.right;
                    } else {
                        if (root.right == null && root.left != null) {
                            root = root.left;
                        } else {
                            if (root.left.right == null) {
                                root.left.right = root.right;
                                root = root.left;
                            } else {
                                BinaryNode leafRight = getLeafRight(root.left);
                                root = leafRight.right;
                                if (leafRight.right.left != null) {
                                    // This leaf has a left
                                    leafRight.right = leafRight.right.left;
                                }
                                leafRight.right = null;
                                root.left = leafRight;
                                root.right = prevRoot.right;
                            }
                        }
                    }
                }
            } else {
                int pathFvalue = pathNode.fValue;
                removeNode(root, pathNode, pathFvalue);
            }
            totalNodes--;
        }

        private void removeNode(BinaryNode node, PathNode pathNode, int pathFvalue) {
            if (pathFvalue <= node.pathNode.fValue) {
                if (node.left != null) {
                    if (node.left.pathNode == pathNode) {
                        BinaryNode del = node.left;
                        if (del.left == null && del.right == null) {
                            node.left = null;
                        } else {
                            if (del.left == null && del.right != null) {
                                node.left = del.right;
                            } else {
                                if (del.right == null && del.left != null) {
                                    node.left = del.left;
                                } else {
                                    if (del.left.right == null) {
                                        del.left.right = del.right;
                                        node.left = del.left;
                                    } else {
                                        BinaryNode leafRight = getLeafRight(del.left);
                                        node.left = leafRight.right;
                                        if (leafRight.right.left != null) {
                                            leafRight.right = leafRight.right.left;
                                        }
                                        leafRight.right = null;
                                        node.left.left = leafRight;
                                        node.left.right = del.right;
                                    }
                                }
                            }
                        }
                    } else {
                        removeNode(node.left, pathNode, pathFvalue);
                    }
                }
            } else {
                if (node.right != null) {
                    if (node.right.pathNode == pathNode) {
                        BinaryNode del = node.right;
                        if (del.left == null && del.right == null) {
                            node.right = null;
                        } else {
                            if (del.left == null && del.right != null) {
                                node.right = del.right;
                            } else {
                                if (del.right == null && del.left != null) {
                                    node.right = del.left;
                                } else {
                                    if (del.left.right == null) {
                                        del.left.right = del.right;
                                        node.right = del.left;
                                    } else {
                                        BinaryNode leafRight = getLeafRight(del.left);
                                        node.right = leafRight.right;
                                        if (leafRight.right.left != null) {
                                            leafRight.right = leafRight.right.left;
                                        }
                                        leafRight.right = null;
                                        node.right.left = leafRight;
                                        node.right.right = del.right;
                                    }
                                }
                            }
                        }
                    } else {
                        removeNode(node.right, pathNode, pathFvalue);
                    }
                }
            }
        }

        private BinaryNode getLeafRight(BinaryNode node) {
            if (node.right.right == null)
                return node;
            else
                return getLeafRight(node.right);
        }

        public int getTotalHeight() {
            return getHeight(root);
        }

        private int getHeight(BinaryNode node) {
            if (node == null) return 0;
            else {
                return 1 + (int)Mathf.Max(getHeight(node.left), getHeight(node.right));
            }
        }

        public PathNode GetSmallest() {
            PathNode pathNode = null;
            int count = 0;
            try {
                pathNode = getSmallest(root, ref count);
            } catch {
                Debug.LogError("GetSmallest: " + count);
            }
            return pathNode;
        }

        private PathNode getSmallest(BinaryNode node, ref int count) {
            if (node == null) return null;
            if (node.left != null) {
                count++;
                return getSmallest(node.left, ref count);
            } else {
                return node.pathNode;
            }
        }
    }
}