# Servarr Key Writer

Allows you to overwrite the API Key of *Arr systems. By writing it to their config xml before booting.

## Use Case

In a kubernetes environment you would want your API Key to come from a central secret store, allowing you to connect *Arr system to other pods running in your cluster.

## Example

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: radarr
spec:
  replicas: 1
  selector:
    matchLabels:
      app: radarr
  strategy:
    type: Recreate
  template:
    metadata:
      labels:
        app: radarr
    spec:
      serviceAccountName: radarr
      containers:
        - name: radarr
          image: linuxserver/radarr:latest
          volumeMounts:
            - name: radarr-config
              mountPath: /config
        - name: key-writer
          image: ghcr.io/beschoenen/servarrkeywriter:v1.0.0
          env:
            - name: ARR_HOST
              value: http://localhost:7878
            - name: SECRET_NAME
              value: radarr-api-key
          volumeMounts:
            - name: radarr-config
              mountPath: /config
      volumes:
        - name: radarr-config
          persistentVolumeClaim:
            claimName: radarr
---
apiVersion: v1
kind: ServiceAccount
metadata:
  name: radarr
---
apiVersion: rbac.authorization.k8s.io/v1
kind: Role
metadata:
  name: radarr-key-writer
rules:
  - apiGroups:
      - ""
    resources:
      - secrets
    verbs:
      - watch
      - list
      - get
---
apiVersion: rbac.authorization.k8s.io/v1
kind: RoleBinding
metadata:
  name: radarr-key-writer
roleRef:
  apiGroup: rbac.authorization.k8s.io
  kind: Role
  name: radarr-key-writer
subjects:
  - kind: ServiceAccount
    name: radarr
    namespace: default
```